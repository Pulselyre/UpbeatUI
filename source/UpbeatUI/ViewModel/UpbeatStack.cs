/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Represents a stack of ViewModels and provides methods and commands for controlling them.
    /// </summary>
    public partial class UpbeatStack : BaseViewModel, IOpensViewModels, IDisposable
    {
        private delegate object ViewModelCreator(IUpbeatService upbeatService, object parameters);

        private readonly ObservableCollection<object> _openViewModels = new ObservableCollection<object>();
        private readonly IDictionary<object, UpbeatService> _openViewModelServices = new Dictionary<object, UpbeatService>();
        private readonly bool _updateOnRender;
        private readonly IDictionary<Type, ViewModelCreator> _viewModelCreators = new Dictionary<Type, ViewModelCreator>();
        private readonly IDictionary<Type, Type> _viewModelControlMappings = new Dictionary<Type, Type>();
        private Action<Type> _autoMapper;

        /// <summary>
        /// Initializes an empty <see cref="UpbeatStack"/>.
        /// </summary>
        public UpbeatStack(bool updateOnRender = true)
        {
            _updateOnRender = updateOnRender;
            ViewModels = new ReadOnlyObservableCollection<object>(_openViewModels);
            RemoveTopViewModelCommand = new DelegateCommand(RemoveTopViewModelAsync, CanRemoveTopViewModel);
            if (_updateOnRender)
                CompositionTarget.Rendering += UpdateViewModelProperties;
        }

        /// <summary>
        /// Gets the <see cref="UpbeatStack"/>'s currently defined mappings between ViewModels and <see cref="UIElement"/> (Views).
        /// </summary>
        public IReadOnlyDictionary<Type, Type> ViewModelControlMappings => null;
        /// <summary>
        /// Gets the <see cref="UpbeatStack"/>'s current ViewModels.
        /// </summary>
        public INotifyCollectionChanged ViewModels { get; }
        /// <summary>
        /// Gets the count of the <see cref="UpbeatStack"/>'s current ViewModels.
        /// </summary>
        public int Count { get { return _openViewModels.Count; } }
        /// <summary>
        /// Gets a command to remove the top (active) ViewModel.
        /// </summary>
        public ICommand RemoveTopViewModelCommand { get; }
        /// <summary>
        /// Gets or sets an <see cref="Action"/> callback that the <see cref="UpbeatStack"/> will execute when it is empty of ViewModels.
        /// </summary>
        public Action ViewModelsEmptyCallback { get; set; }

        public void Dispose()
        {
            if (_updateOnRender)
                CompositionTarget.Rendering -= UpdateViewModelProperties;
            foreach (var upbeatViewModel in _openViewModels.OfType<IDisposable>().Reverse())
                upbeatViewModel.Dispose();
        }

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="viewModelCreator">The delegate that will executed to create the ViewModel from an <see cref="IUpbeatService"/> and <typeparamref name="TParameters"/>.</param>
        public void MapViewModel<TParameters, TViewModel, TView>(
            Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
            where TView : UIElement
        {
            if (viewModelCreator == null)
                throw new ArgumentNullException(nameof(viewModelCreator));
            MapViewModel(typeof(TParameters), typeof(TViewModel), typeof(TView),
                (service, parameters) => viewModelCreator(service, (TParameters)parameters));
        }

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type. The <see cref="IServiceProvider"/> will be used to resolve dependencies when creating the <typeparamref name="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="upbeatStack">The <see cref="UpbeatStack"/> to define the mapping on.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        public void MapViewModel<TParameters, TViewModel, TView>(IServiceProvider serviceProvider)
            where TView : UIElement =>
            MapViewModel(serviceProvider, typeof(TParameters), typeof(TViewModel), typeof(TView));

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
                    service => _viewModelCreators[parametersType](service, parameters),
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

        /// <summary>
        /// Sets the <see cref="UpbeatStack"/> to automatically map Parameters <see cref="Type"/>s to ViewModel <see cref="Type"/>s and View <see cref="Type"/>s using the default conventions.
        /// <para>Parameters class names must follow the pattern of: "{BaseNamespace}.ViewModel.{Name}ViewModel+Parameters" (The Parameters class must be a public nested class of the ViewModel class).</para>
        /// <para>ViewModel class names must follow the pattern of: "{BaseNamespace}.ViewModel.{Name}ViewModel".</para>
        /// <para>View class names must follow the pattern of: "{BaseNamespace}.View.{Name}Control".</para>
        /// <para>For example: "Demo.ViewModel.MessageViewModel+Parameters", "Demo.ViewModel.MessageViewModel", and "Demo.View.MessageControl".</para>
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        public void SetDefaultViewModelLocators(IServiceProvider serviceProvider) =>
            SetViewModelLocators(serviceProvider,
                (String parametersTypeString) => parametersTypeString.Replace("+Parameters", ""),
                (String parametersTypeString) => parametersTypeString.Replace("ViewModel+Parameters", "Control").Replace(".ViewModel.", ".View."));

        /// <summary>
        /// Sets delegates the <see cref="UpbeatStack"/> can use to automatically map a <see cref="string"/> representation of a Parameters <see cref="Type"/> to <see cref="string"/> represetantions of a ViewModel <see cref="Type"/> and a View <see cref="Type"/>.
        /// <para>Note: each <see cref="string"/> representation is a <see cref="Type.AssemblyQualifiedName"/></para>
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        /// <param name="parameterToViewModelLocator">A delegate to identify a <see cref="string"/> representation of a ViewModel <see cref="Type"/> from a <see cref="string"/> represetnation of a Parameters <see cref="Type"/>.
        /// <para>Note: each <see cref="string"/> representation is a <see cref="Type.AssemblyQualifiedName"/></para></param>
        /// <param name="parameterToViewLocator">A delegate to identify a <see cref="string"/> represetnation of a View <see cref="Type"/> from a <see cref="string"/> represetnation of a Parameters <see cref="Type"/>.
        /// <para>Note: The input <see cref="Type"/> is for the Parameters in the mapping, not the ViewModel.
        /// <para>Note: each <see cref="string"/> representation is a <see cref="Type.AssemblyQualifiedName"/></para></para></param>
        public void SetViewModelLocators(IServiceProvider serviceProvider,
                                         Func<string, string> parameterToViewModelLocator,
                                         Func<string, string> parameterToViewLocator) =>
            SetViewModelLocators(serviceProvider,
                (Type parametersType) => Type.GetType(parameterToViewModelLocator(parametersType.AssemblyQualifiedName)),
                (Type parametersType) => Type.GetType(parameterToViewLocator(parametersType.AssemblyQualifiedName)));

        /// <summary>
        /// Sets delegates the <see cref="UpbeatStack"/> can use to automatically map a Parameters <see cref="Type"/> to a ViewModel <see cref="Type"/> and a View <see cref="Type"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        /// <param name="parameterToViewModelLocator">A delegate to locate a ViewModel <see cref="Type"/> from a Parameters <see cref="Type"/>.</param>
        /// <param name="parameterToViewLocator">A delegate to locate a View <see cref="Type"/> from a Parameters <see cref="Type"/>.
        /// <para>Note: The input <see cref="Type"/> represents the Parameters in the mapping, not the ViewModel.</para></param>
        public void SetViewModelLocators(IServiceProvider serviceProvider,
                                         Func<Type, Type> parameterToViewModelLocator,
                                         Func<Type, Type> parameterToViewLocator)
        {
            _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _ = parameterToViewModelLocator ?? throw new ArgumentNullException(nameof(parameterToViewModelLocator));
            _ = parameterToViewLocator ?? throw new ArgumentNullException(nameof(parameterToViewLocator));
            _autoMapper = parametersType =>
            {
                var viewModelType = parameterToViewModelLocator(parametersType) ?? throw new InvalidOperationException($"Unable to locate ViewModel Type from Parameters Type: {parametersType.GetType().Name}");
                var viewType = parameterToViewLocator(parametersType) ?? throw new InvalidOperationException($"Unable to locate View Type from Parameters Type: {parametersType.GetType().Name}");
                MapViewModel(serviceProvider, parametersType, viewModelType, viewType);
            };
        }

        /// <summary>
        /// Tries to close and dispose all open ViewModels from the <see cref="UpbeatStack"/>.
        /// </summary>
        /// <returns>A task that represents the process of closing all ViewModels with a result of whether it was successful or not.</returns>
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

        /// <summary>
        /// Executes the <see cref="UpdateViewModelProperties"/> method on each ViewModel that implements the <see cref="IUpdatableViewModel"/> interface. Only executes if the <see cref="UpbeatStack"/> is set to NOT update on render; otherwise is a noop.
        /// </summary>
        public void UpdateViewModelProperties()
        {
            if (!_updateOnRender)
                UpdateViewModelProperties(this, EventArgs.Empty);
        }

        internal Type GetViewTypeFromViewModelType(Type viewModelType) =>
            _viewModelControlMappings.TryGetValue(viewModelType, out var viewType) ? viewType : null;

        private bool CanRemoveTopViewModel()
            => _openViewModels.Count > 0;

        private void MapViewModel(Type parametersType, Type viewModelType, Type viewType, ViewModelCreator viewModelCreator)
        {
            _viewModelCreators[parametersType] = viewModelCreator;
            _viewModelControlMappings[viewModelType] = viewType;
        }

        private void MapViewModel(IServiceProvider serviceProvider, Type parametersType, Type viewModelType, Type viewType)
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
            MapViewModel(parametersType, viewModelType, viewType,
                (service, parameters) => Convert.ChangeType(constructor.Invoke(constructor.GetParameters().Select(
                        p => p.ParameterType == typeof(IUpbeatService) ? service
                            : p.ParameterType == parametersType ? parameters
                            : serviceProvider.GetService(p.ParameterType)).ToArray()), viewModelType));
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
        private void UpdateViewModelProperties(object sender, EventArgs e)
        {
            foreach (var viewModel in _openViewModels)
                _openViewModelServices[viewModel].Update();
        }
    }
}
