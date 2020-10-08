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
    public partial class UpbeatStack : BaseViewModel, IDisposable
    {
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

        /// <summary>
        /// Adds a new IUpbeatViewModel to the UpbeatStack.
        /// </summary>
        /// <param name="upbeatViewModelCreator">A delegate that accepts an IUpbeatViewModelService as a parameter and returns a new IUpbeatViewModel.</param>
        public void OpenUpbeatViewModel(UpbeatViewModelCreator upbeatViewModelCreator) =>
            OpenUpbeatViewModel(upbeatViewModelCreator, null);

        /// <summary>
        /// Adds a new IUpbeatViewModel to the UpbeatStack and executes a callback after that IUpbeatViewModel closes.
        /// </summary>
        /// <param name="upbeatViewModelCreator">A delegate that accepts an IUpbeatViewModelService as a parameter and returns a new IUpbeatViewModel.</param>
        /// <param name="closeCallback">A delegate for the UpbeatStack to execute after the IUpbeatViewModel closes.</param>
        public void OpenUpbeatViewModel(UpbeatViewModelCreator upbeatViewModelCreator, Action closeCallback)
        {
            var upbeatViewModelService = new UpbeatService(
                OpenUpbeatViewModel, CloseUpbeatViewModel, closeCallback,
                c => _upbeatViewModels.Last() == c);
            using (var d = new UpbeatServiceDeferrer(upbeatViewModelService))
            {
                var upbeatViewModel = upbeatViewModelService.CreateUpbeatViewModel(upbeatViewModelCreator);
                _upbeatViewModelServices[upbeatViewModel] = upbeatViewModelService;
                _upbeatViewModels.Add(upbeatViewModel);
            }
        }

        /// <summary>
        /// Adds a new IUpbeatViewModel to the UpbeatStack and returns a Task that ends after the IUpbeatViewModel closes.
        /// </summary>
        /// <param name="upbeatViewModelCreator">A delegate that accepts an IUpbeatViewModelService as a parameter and returns a new IUpbeatViewModel.</param>
        /// <returns>A Task that ends after the IUpbeatViewModel closes.</returns>
        public async Task OpenUpbeatViewModelAsync(UpbeatViewModelCreator upbeatViewModelCreator)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            OpenUpbeatViewModel(
                upbeatViewModelCreator,
                () => taskCompletionSource.SetResult(true));
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
        /// Defines a mapping between an IUpbeatViewModel Type and a Control (View) Type.
        /// </summary>
        /// <param name="upbeatViewModelType">The Type of the IUpbeatViewModel.</param>
        /// <param name="viewType">The Type of the Control (View).</param>
        public void SetUpbeatViewModelControlMapping(Type upbeatViewModelType, Type viewType)
        {
            if (upbeatViewModelType == null || viewType == null)
                throw new ArgumentNullException();
            if (!typeof(IUpbeatViewModel).IsAssignableFrom(upbeatViewModelType))
                throw new ArgumentException($"{nameof(upbeatViewModelType)} must implement the {typeof(IUpbeatViewModel).Name} interface.");
            if (!typeof(UIElement).IsAssignableFrom(viewType))
                throw new ArgumentException($"{nameof(viewType)} must extend the {typeof(UIElement).Name} class.");
            _upbeatViewModelControlMappings[upbeatViewModelType] = viewType;
        }

        /// <summary>
        /// Defines multiple mapppings between IUpbeatViewModel Types and Control (View) Types.
        /// </summary>
        /// <param name="mappings">An IDictionary linking IUpbeatViewModel Types and Control (View) Types.</param>
        public void SetUpbeatViewModelControlMappings(IDictionary<Type, Type> mappings)
        {
            foreach (var kvp in mappings)
                SetUpbeatViewModelControlMapping(kvp.Key, kvp.Value);
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
