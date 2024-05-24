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
using System.Windows.Input;
using System.Windows.Media;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Represents a stack of ViewModels and provides methods and commands for controlling them.
    /// </summary>
    public partial class UpbeatStack : IUpbeatStack, IDisposable
    {
        /// <summary>
        /// A method signature to create a new ViewModel.
        /// </summary>
        /// <param name="upbeatService">The <see cref="IUpbeatService"/> instance for the ViewModel.</param>
        /// <param name="parameters">The parameters object for the ViewModel.</param>
        /// <returns></returns>
        protected delegate object ViewModelInstantiator(IUpbeatService upbeatService, object parameters);

        private readonly ObservableCollection<object> _openViewModels = new ObservableCollection<object>();
        private readonly Dictionary<object, UpbeatService> _openViewModelServices = new Dictionary<object, UpbeatService>();
        private readonly bool _updateOnRender;
        private bool _disposed;

        /// <summary>
        /// Initializes an empty <see cref="UpbeatStack"/>.
        /// </summary>
        /// <param name="updateOnRender">True to have the <see cref="UpbeatStack"/> execute any update callback <see cref="Action"/>s registered by ViewModels on each WPF frame render; false to only execute update callbacks manually (with <see cref="UpdateViewModelProperties()"/>).</param>
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

        /// <inheritdoc/>
        public event EventHandler ViewModelsEmptied;

        /// <inheritdoc/>
        public int Count => _openViewModels.Count;
        /// <inheritdoc/>
        public ICommand RemoveTopViewModelCommand { get; }
        /// <inheritdoc/>
        public INotifyCollectionChanged ViewModels { get; }
        /// <summary>
        /// A <see cref="Dictionary{K,V}"/> mapping ViewModel <see cref="Type"/>s to <see cref="ViewModelInstantiator"/>s.
        /// </summary>
        protected Dictionary<Type, ViewModelInstantiator> ViewModelInstantiators { get; } = new Dictionary<Type, ViewModelInstantiator>();

        /// <summary>
        /// Internal method to dispose unmanaged resources.
        /// </summary>
        /// <param name="disposing">Use true if calling from a <see cref="Dispose()"/> method, false if from a destructor.</param>
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

        /// <summary>
        /// Destructor for orderly disposal.
        /// </summary>
        ~UpbeatStack() => Dispose(false);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public void MapViewModel<TParameters, TViewModel>(
            Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
        {
            if (viewModelCreator == null)
            {
                throw new ArgumentNullException(nameof(viewModelCreator));
            }

            MapViewModel(
                typeof(TParameters),
                (service, parameters) => viewModelCreator(service, (TParameters)parameters));
        }

        /// <inheritdoc/>
        public void OpenViewModel<TParameters>(TParameters parameters) =>
            OpenViewModel(parameters, null);

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public Task OpenViewModelAsync<TParameters>(TParameters parameters)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            OpenViewModel(parameters, () => taskCompletionSource.SetResult(true));
            return taskCompletionSource.Task;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
            ViewModelInstantiator viewModelCreator) =>
            ViewModelInstantiators[parametersType] = viewModelCreator;

        private async Task<bool> TryRemoveViewModelAsync(object viewModel)
        {
            var viewModelService = _openViewModelServices[viewModel];
            if (viewModelService.Closing)
            {
                return false;
            }
            viewModelService.Closing = true;
            if (await _openViewModelServices[viewModel].OkToCloseAsync().ConfigureAwait(true))
            {
                (viewModel as IDisposable)?.Dispose();
                _ = _openViewModels.Remove(viewModel);
                _ = _openViewModelServices.Remove(viewModel);
                viewModelService.ClosedCallback?.Invoke();
                if (_openViewModels.Count == 0)
                {
                    ViewModelsEmptied?.Invoke(this, EventArgs.Empty);
                }
                return true;
            }
            viewModelService.Closing = false;
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
