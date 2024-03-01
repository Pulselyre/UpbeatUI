/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
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
    public partial class UpbeatStack : IUpbeatStack, IDisposable
    {
        protected delegate object ViewModelInstantiator(IUpbeatService upbeatService, object parameters);

        private readonly ObservableCollection<object> _openViewModels = new ObservableCollection<object>();
        private readonly Dictionary<object, UpbeatService> _openViewModelServices = new Dictionary<object, UpbeatService>();
        private readonly bool _updateOnRender;
        private bool _disposed;

        /// <summary>
        /// Initializes an empty <see cref="UpbeatStack"/>.
        /// </summary>
        /// <param name="updateOnRender">True to have the <see cref="UpbeatStack"/> execute any UppdateCallback <see cref="Action"/>s registered by ViewModels on each WPF frame render; false to only execute UpdateCallback <see cref="Action"/>s manually.</param>
        public UpbeatStack(bool updateOnRender = true)
        {
            _updateOnRender = updateOnRender;
            ViewModels = new ReadOnlyObservableCollection<object>(_openViewModels);
            RemoveTopViewModelCommand = new RelayCommand(
                () => TryRemoveViewModelAsync(_openViewModels.Last()),
                CanRemoveTopViewModel, singleExecution: false);
            if (_updateOnRender)
            {
                CompositionTarget.Rendering += UpdateViewModelProperties;
            }
        }

        public event EventHandler ViewModelsEmptied;

        public int Count => _openViewModels.Count;
        public ICommand RemoveTopViewModelCommand { get; }
        public INotifyCollectionChanged ViewModels { get; }
        protected Dictionary<Type, Type> ViewModelControlMappings { get; } = new Dictionary<Type, Type>();
        protected Dictionary<Type, ViewModelInstantiator> ViewModelInstantiators { get; } = new Dictionary<Type, ViewModelInstantiator>();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var upbeatViewModel in _openViewModels.OfType<IDisposable>().Reverse())
                    {
                        upbeatViewModel.Dispose();
                    }
                    _openViewModels.Clear();
                }
                if (_updateOnRender)
                {
                    CompositionTarget.Rendering -= UpdateViewModelProperties;
                }
            }
            _disposed = true;
        }

        ~UpbeatStack() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Type GetViewTypeFromViewModelType(Type viewModelType) =>
            ViewModelControlMappings.TryGetValue(viewModelType, out var viewType) ? viewType : null;

        public void MapViewModel<TParameters, TViewModel, TView>(
            Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
            where TView : UIElement
        {
            if (viewModelCreator == null)
            {
                throw new ArgumentNullException(nameof(viewModelCreator));
            }

            MapViewModel(typeof(TParameters), typeof(TViewModel), typeof(TView),
                (service, parameters) => viewModelCreator(service, (TParameters)parameters));
        }

        public void OpenViewModel<TParameters>(TParameters parameters) =>
            OpenViewModel(parameters, null);

        public virtual void OpenViewModel<TParameters>(TParameters parameters, Action closedCallback)
        {
            var parametersType = parameters.GetType();
            var upbeatViewModelService = new UpbeatService(_updateOnRender, OpenViewModel, closedCallback);
            using var d = new UpbeatServiceDeferrer(upbeatViewModelService);
            var viewModel = upbeatViewModelService.Activate(
                service => ViewModelInstantiators[parametersType](service, parameters),
                vm => _openViewModels.Last() == vm,
                async vm => await TryRemoveViewModelAsync(vm).ConfigureAwait(true));
            _openViewModelServices[viewModel] = upbeatViewModelService;
            _openViewModels.Add(viewModel);
        }

        public Task OpenViewModelAsync<TParameters>(TParameters parameters)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            OpenViewModel(parameters, () => taskCompletionSource.SetResult(true));
            return taskCompletionSource.Task;
        }

        public async Task<bool> TryCloseAllViewModelsAsync()
        {
            foreach (var viewModel in _openViewModels.Reverse())
            {
                if (!await TryRemoveViewModelAsync(viewModel).ConfigureAwait(true))
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateViewModelProperties()
        {
            if (!_updateOnRender)
            {
                UpdateViewModelProperties(this, EventArgs.Empty);
            }
        }

        private bool CanRemoveTopViewModel()
            => _openViewModels.Count > 0;

        private void MapViewModel(
            Type parametersType,
            Type viewModelType,
            Type viewType,
            ViewModelInstantiator viewModelCreator)
        {
            ViewModelInstantiators[parametersType] = viewModelCreator;
            ViewModelControlMappings[viewModelType] = viewType;
        }

        private void RemoveViewModel(object viewModel)
        {
            (viewModel as IDisposable)?.Dispose();
            _ = _openViewModels.Remove(viewModel);
            var closedCallback = _openViewModelServices[viewModel].ClosedCallback;
            _ = _openViewModelServices.Remove(viewModel);
            closedCallback?.Invoke();
            if (_openViewModels.Count == 0)
            {
                ViewModelsEmptied?.Invoke(this, EventArgs.Empty);
            }
        }

        private async Task<bool> TryRemoveViewModelAsync(object viewModel)
        {
            if (await _openViewModelServices[viewModel].OkToCloseAsync().ConfigureAwait(true))
            {
                RemoveViewModel(viewModel);
                return true;
            }
            return false;
        }

        private void UpdateViewModelProperties(object sender, EventArgs e)
        {
            foreach (var viewModel in _openViewModels)
            {
                _openViewModelServices[viewModel].Update();
            }
        }
    }
}
