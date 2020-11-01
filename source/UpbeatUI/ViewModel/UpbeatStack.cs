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
    public partial class UpbeatStack : BaseViewModel, IOpensViewModels, IOpensUpbeatViewModels, IDisposable
    {
        private delegate object UpbeatViewModelCreator(IUpbeatService upbeatService, object parameters);

        private readonly ObservableCollection<object> _openViewModels = new ObservableCollection<object>();
        private readonly IDictionary<object, UpbeatService> _openViewModelServices = new Dictionary<object, UpbeatService>();
        private readonly bool _updateOnRender;
        private readonly IDictionary<Type, UpbeatViewModelCreator> _viewModelCreators = new Dictionary<Type, UpbeatViewModelCreator>();
        private readonly IDictionary<Type, Type> _viewModelControlMappings = new Dictionary<Type, Type>();

        /// <summary>
        /// Initializes an empty <see cref="UpbeatStack"/>.
        /// </summary>
        public UpbeatStack(bool updateOnRender = true)
        {
            _updateOnRender = updateOnRender;
            ViewModelControlMappings = new ReadOnlyDictionary<Type, Type>(_viewModelControlMappings);
            ViewModels = new ReadOnlyObservableCollection<object>(_openViewModels);
            RemoveTopViewModelCommand = new DelegateCommand(RemoveTopViewModelAsync, CanRemoveTopViewModel);
            if (_updateOnRender)
                CompositionTarget.Rendering += UpdateViewModelProperties;
        }

        /// <summary>
        /// Gets the <see cref="UpbeatStack"/>'s currently defined mappings between ViewModels and <see cref="UIElement"/> (Views).
        /// </summary>
        [Obsolete("Renamed to ViewModelControlMappings. This property will be removed in UpbeatUI 3.0.")]
        public IReadOnlyDictionary<Type, Type> UpbeatViewModelControlMappings => ViewModelControlMappings;
        /// <summary>
        /// Gets the <see cref="UpbeatStack"/>'s currently defined mappings between ViewModels and <see cref="UIElement"/> (Views).
        /// </summary>
        public IReadOnlyDictionary<Type, Type> ViewModelControlMappings { get; }
        /// <summary>
        /// Gets the <see cref="UpbeatStack"/>'s current ViewModels.
        /// </summary>
        [Obsolete("Renamed to ViewModels. This property will be removed in UpbeatUI 3.0.")]
        public INotifyCollectionChanged UpbeatViewModels => ViewModels;
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
        [Obsolete("Renamed to RemoveTopViewModelCommand. This property will be removed in UpbeatUI 3.0.")]
        public ICommand RemoveTopUpbeatViewModelCommand => RemoveTopViewModelCommand;
        /// <summary>
        /// Gets a command to remove the top (active) ViewModel.
        /// </summary>
        public ICommand RemoveTopViewModelCommand { get; }
        /// <summary>
        /// Gets or sets an <see cref="Action"/> callback that the <see cref="UpbeatStack"/> will execute when it is empty of ViewModels.
        /// </summary>
        [Obsolete("Renamed to ViewModelsEmptyCallback. This property will be removed in UpbeatUI 3.0.")]
        public Action UpbeatViewModelsEmptyCallback
        {
            get => ViewModelsEmptyCallback;
            set => ViewModelsEmptyCallback = value;
        }
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
        [Obsolete("Renamed to MapViewModel. This method will be removed in UpbeatUI 3.0.")]
        public void MapUpbeatViewModel<TParameters, TViewModel, TView>(
            Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
            where TView : UIElement =>
            MapViewModel<TParameters, TViewModel, TView>(viewModelCreator);

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
            _viewModelCreators[typeof(TParameters)] = (service, parameters) => viewModelCreator(service, (TParameters)parameters);
            _viewModelControlMappings[typeof(TViewModel)] = typeof(TView);
        }

        [Obsolete("Renamed to OpenViewModelAsync. This method will be removed in UpbeatUI 3.0.")]
        public void OpenUpbeatViewModel<TParameters>(TParameters parameters) =>
            OpenViewModel(parameters);

        [Obsolete("Renamed to OpenViewModelAsync. This method will be removed in UpbeatUI 3.0.")]
        public void OpenUpbeatViewModel<TParameters>(TParameters parameters, Action closedCallback) =>
            OpenViewModel(parameters, closedCallback);

        [Obsolete("Renamed to OpenViewModelAsync. This method will be removed in UpbeatUI 3.0.")]
        public Task OpenUpbeatViewModelAsync<TParameters>(TParameters parameters) =>
            OpenViewModelAsync(parameters);

        public void OpenViewModel<TParameters>(TParameters parameters) =>
            OpenViewModel(parameters, null);

        public void OpenViewModel<TParameters>(TParameters parameters, Action closedCallback)
        {
            var upbeatViewModelService = new UpbeatService(_updateOnRender, OpenViewModel, closedCallback);
            using (var d = new UpbeatServiceDeferrer(upbeatViewModelService))
            {
                var viewModel = upbeatViewModelService.Activate(
                    service => _viewModelCreators[parameters.GetType()](service, parameters),
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
        /// Closes and disposes all ViewModels from the <see cref="UpbeatStack"/>.
        /// </summary>
        [Obsolete("Use TryCloseAllViewModelsAsync instead, which gives ViewModels a chance to cancel closing (e.g., to save unsaved work). This method will be removed in UpbeatUI 3.0.")]
        public void RemoveAllUpbeatViewModels()
        {
            foreach (var viewModel in _openViewModels.Reverse())
                RemoveViewModel(viewModel);
        }

        /// <summary>
        /// Executes the <see cref="UpdateViewModelProperties"/> method on each ViewModel that implements the <see cref="IUpdatableViewModel"/> interface. This is a convenience method that can easily be subscribed to the <see cref="CompositionTarget.Rendering"/> event, which fires for each frame render.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        [Obsolete("The UpbeatStack will automatically execute UpdateViewModelProperties on each frame render unless it is constructed with 'updateOnRender' as false. This handler is now a noop and will be removed in UpbeatUI 3.0.")]
        public void RenderingHandler(object sender, EventArgs e)
        {
            return;
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

        private bool CanRemoveTopViewModel()
            => _openViewModels.Count > 0;

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
