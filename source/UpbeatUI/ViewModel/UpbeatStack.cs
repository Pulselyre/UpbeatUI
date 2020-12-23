/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Represents a stack of ViewModels and provides methods and commands for controlling them.
    /// </summary>
    public partial class UpbeatStack : BaseViewModel, IUpbeatStack, IDisposable
    {
        private delegate object Instantiator(IUpbeatService upbeatService, object parameters);

        private readonly IDictionary<Type, Instantiator> _instantiators = new Dictionary<Type, Instantiator>();
        private readonly ObservableCollection<object> _openViewModels = new ObservableCollection<object>();
        private readonly IDictionary<object, UpbeatService> _openViewModelServices = new Dictionary<object, UpbeatService>();
        private readonly bool _updateOnRender;
        private readonly IDictionary<Type, Type> _viewModelCreators = new Dictionary<Type, Type>();
        private readonly IDictionary<Type, Type> _viewModelControlMappings = new Dictionary<Type, Type>();
        private Action<Type> _autoMapper;

        /// <summary>
        /// Initializes an empty <see cref="UpbeatStack"/>.
        /// </summary>
        public UpbeatStack(bool updateOnRender = true)
        {
            _updateOnRender = updateOnRender;
            ViewModels = new ReadOnlyObservableCollection<object>(_openViewModels);
            RemoveTopViewModelCommand = new DelegateCommand(RemoveTopViewModelAsync, CanRemoveTopViewModel, singleExecution: false);
            if (_updateOnRender)
                CompositionTarget.Rendering += UpdateViewModelProperties;
        }

        public int Count { get { return _openViewModels.Count; } }
        public ICommand RemoveTopViewModelCommand { get; }
        public INotifyCollectionChanged ViewModels { get; }
        public Action ViewModelsEmptyCallback { get; set; }

        public void Dispose()
        {
            if (_updateOnRender)
                CompositionTarget.Rendering -= UpdateViewModelProperties;
            foreach (var upbeatViewModel in _openViewModels.OfType<IDisposable>().Reverse())
                upbeatViewModel.Dispose();
        }

        public void MapViewModel<TParameters, TViewModel, TView>(
            Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
            where TView : UIElement
        {
            if (viewModelCreator == null)
                throw new ArgumentNullException(nameof(viewModelCreator));
            MapViewModel(typeof(TParameters), typeof(TViewModel), typeof(TView),
                (service, parameters) => viewModelCreator(service, (TParameters)parameters));
        }

        public void MapViewModel<TParameters, TViewModel, TView>(IServiceProvider serviceProvider, bool allowNullServices = false)
            where TView : UIElement =>
            MapViewModel(serviceProvider, typeof(TParameters), typeof(TViewModel), typeof(TView), allowNullServices);

        public void OpenViewModel<TParameters>(TParameters parameters) =>
            OpenViewModel(parameters, null);

        public void OpenViewModel<TParameters>(TParameters parameters, Action closedCallback)
        {
            var parametersType = parameters.GetType();
            if (!_viewModelCreators.ContainsKey(parametersType))
                _autoMapper?.Invoke(parametersType);
            var upbeatViewModelService = new UpbeatService(_updateOnRender, OpenViewModel, closedCallback);
            using (var d = new UpbeatServiceDeferrer(upbeatViewModelService))
            {
                var viewModel = upbeatViewModelService.Activate(
                    service => _instantiators[_viewModelCreators[parametersType]](service, parameters),
                    vm => _openViewModels.Last() == vm,
                    vm => RemoveViewModel(vm));
                _openViewModelServices[viewModel] = upbeatViewModelService;
                _openViewModels.Add(viewModel);
            }
        }

        public Task OpenViewModelAsync<TParameters>(TParameters parameters)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            OpenViewModel(parameters, () => taskCompletionSource.SetResult(true));
            return taskCompletionSource.Task;
        }

        public void SetDefaultViewModelLocators(IServiceProvider serviceProvider, bool allowNullServices = false) =>
            SetViewModelLocators(serviceProvider,
                (String parametersTypeString) => parametersTypeString.Replace("+Parameters", ""),
                (String parametersTypeString) => parametersTypeString.Replace("ViewModel+Parameters", "Control").Replace(".ViewModel.", ".View."),
                allowNullServices);

        public void SetViewModelLocators(IServiceProvider serviceProvider,
                                         Func<string, string> parameterToViewModelLocator,
                                         Func<string, string> parameterToViewLocator,
                                         bool allowNullServices = false) =>
            SetViewModelLocators(serviceProvider,
                (Type parametersType) => Type.GetType(parameterToViewModelLocator(parametersType.AssemblyQualifiedName)),
                (Type parametersType) => Type.GetType(parameterToViewLocator(parametersType.AssemblyQualifiedName)),
                allowNullServices);

        public void SetViewModelLocators(IServiceProvider serviceProvider,
                                         Func<Type, Type> parameterToViewModelLocator,
                                         Func<Type, Type> parameterToViewLocator,
                                         bool allowNullServices = false)
        {
            _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _ = parameterToViewModelLocator ?? throw new ArgumentNullException(nameof(parameterToViewModelLocator));
            _ = parameterToViewLocator ?? throw new ArgumentNullException(nameof(parameterToViewLocator));
            _autoMapper = parametersType =>
            {
                var viewModelType = parameterToViewModelLocator(parametersType) ?? throw new InvalidOperationException($"Unable to locate ViewModel Type from Parameters Type: {parametersType.GetType().Name}");
                var viewType = parameterToViewLocator(parametersType) ?? throw new InvalidOperationException($"Unable to locate View Type from Parameters Type: {parametersType.GetType().Name}");
                MapViewModel(serviceProvider, parametersType, viewModelType, viewType, allowNullServices);
            };
        }

        public async Task<bool> TryCloseAllViewModelsAsync()
        {
            foreach (var viewModel in _openViewModels.Reverse())
            {
                if (await _openViewModelServices[viewModel].OkToCloseAsync())
                    RemoveViewModel(viewModel);
                else
                    return false;
            }
            return true;
        }

        public void UpdateViewModelProperties()
        {
            if (!_updateOnRender)
                UpdateViewModelProperties(this, EventArgs.Empty);
        }

        internal Type GetViewTypeFromViewModelType(Type viewModelType) =>
            _viewModelControlMappings.TryGetValue(viewModelType, out var viewType) ? viewType : null;

        private bool CanRemoveTopViewModel()
            => _openViewModels.Count > 0;

        private Instantiator CreateInstantiator(ConstructorInfo constructor, Type targetType,
                                                IServiceProvider serviceProvider, bool allowNullServices) =>
            new Instantiator((upbeatService, parameters) => Convert.ChangeType(
                    constructor.Invoke(constructor.GetParameters()
                        .Select(p =>
                        {
                            if (p.ParameterType == typeof(IUpbeatService))
                                return upbeatService;
                            if (p.ParameterType == parameters.GetType())
                                return parameters;
                            var locatedService = serviceProvider.GetService(p.ParameterType);
                            if (locatedService != null)
                                return locatedService;
                            locatedService = ResolveDependency(p.ParameterType, serviceProvider, allowNullServices, upbeatService, parameters);
                            if (locatedService == null && !allowNullServices)
                                throw new InvalidOperationException($"No service for type '{p.ParameterType.FullName}' has been registered.");
                            return locatedService;
                        })
                        .ToArray()),
                    targetType));

        private void MapViewModel(Type parametersType, Type viewModelType, Type viewType, Instantiator viewModelCreator)
        {
            _instantiators[viewModelType] = viewModelCreator;
            _viewModelCreators[parametersType] = viewModelType;
            _viewModelControlMappings[viewModelType] = viewType;
        }

        private void MapViewModel(IServiceProvider serviceProvider, Type parametersType, Type viewModelType, Type viewType, bool allowNullServices = false)
        {
            _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _ = parametersType ?? throw new ArgumentNullException(nameof(parametersType));
            _ = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
            _ = viewType ?? throw new ArgumentNullException(nameof(viewType));
            var constructors = viewModelType.GetConstructors().ToList();
            if (constructors.Count > 1)
                throw new InvalidOperationException($"Type {viewModelType.Name} has more than one constructor.");
            var constructor = constructors[0];
            var serviceType = typeof(IUpbeatService);
            MapViewModel(
                parametersType, viewModelType, viewType,
                CreateInstantiator(constructor, viewModelType, serviceProvider, allowNullServices));
        }

        private void RemoveViewModel(object viewModel)
        {
            (viewModel as IDisposable)?.Dispose();
            _openViewModels.Remove(viewModel);
            var closedCallback = _openViewModelServices[viewModel].ClosedCallback;
            _openViewModelServices.Remove(viewModel);
            closedCallback?.Invoke();
            if (_openViewModels.Count == 0)
                ViewModelsEmptyCallback?.Invoke();
        }

        private async Task RemoveTopViewModelAsync()
        {
            var viewModel = _openViewModels.Last();
            if (await _openViewModelServices[viewModel].OkToCloseAsync())
                RemoveViewModel(viewModel);
            else
                return;
        }

        private object ResolveDependency(Type dependencyType, IServiceProvider serviceProvider, bool allowNullServices, IUpbeatService upbeatService, object parameters)
        {
            if (!_instantiators.TryGetValue(dependencyType, out var instantiator))
            {
                var constructors = dependencyType.GetConstructors().ToList();
                if (constructors.Count > 1)
                    return null;
                var constructor = constructors[0];
                var parameterTypes = constructor.GetParameters().Select(p => p.ParameterType);
                if (parameterTypes.Intersect(new[] { typeof(IUpbeatService), parameters.GetType() }).Count() == 0)
                    return null;
                instantiator = CreateInstantiator(constructor, dependencyType, serviceProvider, allowNullServices);
                _instantiators[dependencyType] = instantiator;
            }
            return instantiator(upbeatService, parameters);
        }

        private void UpdateViewModelProperties(object sender, EventArgs e)
        {
            foreach (var viewModel in _openViewModels)
                _openViewModelServices[viewModel].Update();
        }
    }
}
