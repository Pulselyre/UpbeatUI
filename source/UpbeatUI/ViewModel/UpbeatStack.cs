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

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Represents a stack of IUpbeatViewModels and provides methods and commands for controlling them.
    /// </summary>
    public partial class UpbeatStack : BaseViewModel, IOpensUpbeatViewModels, IDisposable
    {
        private delegate IUpbeatViewModel UpbeatViewModelCreator(IUpbeatService upbeatService, object parameters);

        private IDictionary<Type, UpbeatViewModelCreator> _upbeatViewModelCreators = new Dictionary<Type, UpbeatViewModelCreator>();
        private IDictionary<Type, Type> _upbeatViewModelControlMappings = new Dictionary<Type, Type>();
        private ObservableCollection<IUpbeatViewModel> _upbeatViewModels = new ObservableCollection<IUpbeatViewModel>();
        private IDictionary<IUpbeatViewModel, UpbeatService> _upbeatViewModelServices = new Dictionary<IUpbeatViewModel, UpbeatService>();

        /// <summary>
        /// Initializes an empty UpbeatStack.
        /// </summary>
        public UpbeatStack()
        {
            UpbeatViewModelControlMappings = new ReadOnlyDictionary<Type, Type>(_upbeatViewModelControlMappings);
            UpbeatViewModels = new ReadOnlyObservableCollection<IUpbeatViewModel>(_upbeatViewModels);
            RemoveTopUpbeatViewModelCommand = new DelegateCommand(RemoveTopUpbeatViewModel, CanRemoveTopUpbeatViewModel);
        }

        /// <summary>
        /// Gets the UpbeatStack's currently defined mappings between IUpbeatViewModels and Controls (Views).
        /// </summary>
        public IReadOnlyDictionary<Type, Type> UpbeatViewModelControlMappings { get; }
        /// <summary>
        /// Gets the UpbeatStack's current IUpbeatViewModels.
        /// </summary>
        public INotifyCollectionChanged UpbeatViewModels { get; }
        /// <summary>
        /// Gets the count of the UpbeatStack's current IUpbeatViewModels.
        /// </summary>
        public int Count { get { return _upbeatViewModels.Count; } }
        /// <summary>
        /// Gets a command to remove the top (active) IUpbeatViewModel.
        /// </summary>
        public ICommand RemoveTopUpbeatViewModelCommand { get; }
        /// <summary>
        /// Gets or sets an Action callback that the UpbeatStack will execute when it is empty of IUpbeatViewModels.
        /// </summary>
        public Action UpbeatViewModelsEmptyCallback { get; set; }

        public void Dispose()
        {
            foreach (var upbeatViewModel in _upbeatViewModels.Reverse())
                upbeatViewModel.Dispose();
        }

        public void OpenUpbeatViewModel<TParameters>(TParameters parameters) =>
            OpenUpbeatViewModel(parameters, null);

        public void OpenUpbeatViewModel<TParameters>(TParameters parameters, Action closedCallback)
        {
            var upbeatViewModelService = new UpbeatService(
                OpenUpbeatViewModel, CloseUpbeatViewModel, closedCallback,
                c => _upbeatViewModels.Last() == c);
            using (var d = new UpbeatServiceDeferrer(upbeatViewModelService))
            {
                var upbeatViewModel = upbeatViewModelService.CreateUpbeatViewModel(
                    service => _upbeatViewModelCreators[parameters.GetType()](service, parameters));
                _upbeatViewModelServices[upbeatViewModel] = upbeatViewModelService;
                _upbeatViewModels.Add(upbeatViewModel);
            }
        }

        public async Task OpenUpbeatViewModelAsync<TParameters>(TParameters parameters)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            OpenUpbeatViewModel(parameters, () => taskCompletionSource.SetResult(true));
            await taskCompletionSource.Task;
        }

        /// <summary>
        /// Executes the UpdateViewModelProperties method on each IUpbeatViewModel that implements the IUpdatableViewModel interface.
        /// </summary>
        public void UpdateViewModelProperties()
        {
            foreach (var upbeatViewModel in _upbeatViewModels)
                (upbeatViewModel as IUpdatableViewModel)?.UpdateViewModelProperties();
        }

        /// <summary>
        /// Disposes and removes all IUpbeatViewModels from the UpbeatStack.
        /// </summary>
        public void RemoveAllUpbeatViewModels()
        {
            if (_upbeatViewModels.Count == 0)
                UpbeatViewModelsEmptyCallback?.Invoke();
            else
            {
                var upbeatViewModel = _upbeatViewModels[_upbeatViewModels.Count - 1];
                upbeatViewModel.SignalToClose(() =>
                {
                    RemoveUpbeatViewModel(upbeatViewModel);
                    RemoveAllUpbeatViewModels();
                });
            }
        }

        /// <summary>
        /// Executes the UpdateViewModelProperties method on each IUpbeatViewModel that implements the IUpdatableViewModel interface. This is a convenience method that can easily be subscribed to the CompositionTarget.Rendering event, which fires for each frame render.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        public void RenderingHandler(object sender, EventArgs e) =>
            UpdateViewModelProperties();

        /// <summary>
        /// Defines a mapping between parameters, an IUpbeatViewModel Type and a Control (View) Type.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create IUpbeatViewModels.</typeparam>
        /// <typeparam name="TUpbeatViewModel">The type of the IUpbeatViewModel created from TParameters.</typeparam>
        /// <typeparam name="TView">The Type of the Control (View).</typeparam>
        /// <param name="upbeatViewModelCreator"></param>
        public void MapUpbeatViewModel<TParameters, TUpbeatViewModel, TView>(
            Func<IUpbeatService, TParameters, IUpbeatViewModel> upbeatViewModelCreator)
            where TUpbeatViewModel : IUpbeatViewModel
            where TView : UIElement
        {
            if (upbeatViewModelCreator == null)
                throw new ArgumentNullException(nameof(upbeatViewModelCreator));
            _upbeatViewModelCreators[typeof(TParameters)] = (service, parameters) => upbeatViewModelCreator(service, (TParameters)parameters);
            _upbeatViewModelControlMappings[typeof(TUpbeatViewModel)] = typeof(TView);
        }

        private bool CanRemoveTopUpbeatViewModel()
            => _upbeatViewModels.Count > 0;

        private void CloseUpbeatViewModel(IUpbeatViewModel upbeatViewModel)
        {
            using (var d = new UpbeatServiceDeferrer(_upbeatViewModelServices[upbeatViewModel]))
                RemoveUpbeatViewModel(upbeatViewModel);
        }

        private void RemoveUpbeatViewModel(IUpbeatViewModel upbeatViewModel)
        {
            upbeatViewModel.Dispose();
            _upbeatViewModels.Remove(upbeatViewModel);
            _upbeatViewModelServices[upbeatViewModel].CloseCallback();
            _upbeatViewModelServices.Remove(upbeatViewModel);
        }

        private void RemoveTopUpbeatViewModel()
        {
            var upbeatViewModel = _upbeatViewModels[_upbeatViewModels.Count - 1];
            upbeatViewModel.SignalToClose(() => RemoveUpbeatViewModel(upbeatViewModel));
        }
    }
}
