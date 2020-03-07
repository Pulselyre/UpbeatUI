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

namespace UpbeatUI.Context
{
    /// <summary>
    /// Represents a stack of IContexts (View Models) and provides methods and commands for controlling them.
    /// </summary>
    public partial class ContextStack : ObservableObject, IDisposable
    {
        private IDictionary<Type, Type> _contextControlMappings = new Dictionary<Type, Type>();
        private ObservableCollection<IContext> _contexts = new ObservableCollection<IContext>();
        private IDictionary<IContext, ContextService> _contextServices = new Dictionary<IContext, ContextService>();

        /// <summary>
        /// Initializes an empty ContextStack.
        /// </summary>
        public ContextStack()
        {
            ContextControlMappings = new ReadOnlyDictionary<Type, Type>(_contextControlMappings);
            Contexts = new ReadOnlyObservableCollection<IContext>(_contexts);
            RemoveTopContextCommand = new ObservableCommand(RemoveTopContext, CanRemoveTopContext);
        }

        /// <summary>
        /// Gets the ContextStack's currently defined mappings between IContexts (View Models) and Controls (Views).
        /// </summary>
        public IReadOnlyDictionary<Type, Type> ContextControlMappings { get; }
        /// <summary>
        /// Gets the ContextStack's current IContexts (View Models).
        /// </summary>
        public INotifyCollectionChanged Contexts { get; }
        /// <summary>
        /// Gets the count of the ContextStack's current IContexts (View Models).
        /// </summary>
        public int Count { get { return _contexts.Count; } }
        /// <summary>
        /// Gets a command to remove the top (active) IContext (View Model).
        /// </summary>
        public ICommand RemoveTopContextCommand { get; }
        /// <summary>
        /// Gets or sets an Action callback that the ContextStack will execute when it is empty of IContexts (View Models).
        /// </summary>
        public Action ContextsEmptyCallback { get; set; }

        public void Dispose()
        {
            foreach (var context in _contexts.Reverse())
                context.Dispose();
        }

        /// <summary>
        /// Adds a new IContext (View Model) to the ContextStack.
        /// </summary>
        /// <param name="creator">A delegate that accepts an IContextService as a parameter and returns a new IContext (View Model).</param>
        public void OpenContext(ContextCreator creator) =>
            OpenContext(creator, null);

        /// <summary>
        /// Adds a new IContext (View Model) to the ContextStack and executes a callback after that IContext closes.
        /// </summary>
        /// <param name="creator">A delegate that accepts an IContextService as a parameter and returns a new IContext (View Model).</param>
        /// <param name="closeCallback">A delegate for the ContextStack to execute after the IContext closes.</param>
        public void OpenContext(ContextCreator creator, Action closeCallback)
        {
            var contextService = new ContextService(
                OpenContext, CloseContext, closeCallback,
                c => _contexts.Last() == c);
            using (var d = new ContextServiceDeferrer(contextService))
            {
                var context = contextService.CreateContext(creator);
                _contextServices[context] = contextService;
                _contexts.Add(context);
            }
        }

        /// <summary>
        /// Adds a new IContext (View Model) to the ContextStack and returns a Task that ends after the IContext closes.
        /// </summary>
        /// <param name="creator">A delegate that accepts an IContextService as a parameter and returns a new IContext (View Model).</param>
        /// <returns>A Task that ends after the IContext closes.</returns>
        public async Task OpenContextAsync(ContextCreator contextCreator)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            OpenContext(
                contextCreator,
                () => taskCompletionSource.SetResult(true));
            await taskCompletionSource.Task;
        }

        /// <summary>
        /// Executes the UpdateContexts method on each IContext (View Model) that implements the IUpdatableContext interface.
        /// </summary>
        public void UpdateContextProperties()
        {
            foreach (var context in _contexts)
                (context as IUpdatableContext)?.UpdateContextProperties();
        }

        /// <summary>
        /// Disposes and removes all IContexts (View Models) from the ContextStack.
        /// </summary>
        public void RemoveAllContexts()
        {
            if (_contexts.Count == 0)
                ContextsEmptyCallback?.Invoke();
            else
            {
                var context = _contexts[_contexts.Count - 1];
                context.SignalToClose(() =>
                {
                    RemoveContext(context);
                    RemoveAllContexts();
                });
            }
        }

        /// <summary>
        /// Executes the UpdateContexts method on each IContext (View Model) that implements the IUpdatableContext interface. This is a convenience method that can easily be subscribed to the CompositionTarget.Rendering event, which fires for each frame render.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        public void RenderingHandler(object sender, EventArgs e) =>
            UpdateContextProperties();

        /// <summary>
        /// Defines a mapping between an IContext (View Model) Type and a Control (View) Type.
        /// </summary>
        /// <param name="contextType">The Type of the IContext (View Model).</param>
        /// <param name="viewType">The Type of the Control (View).</param>
        public void SetContextControlMapping(Type contextType, Type viewType)
        {
            if (contextType == null || viewType == null)
                throw new ArgumentNullException();
            if (!typeof(IContext).IsAssignableFrom(contextType))
                throw new ArgumentException("contextType must implement the IContext interface.");
            if (!typeof(UIElement).IsAssignableFrom(viewType))
                throw new ArgumentException("viewType must extend the UIElement class.");
            _contextControlMappings[contextType] = viewType;
        }

        /// <summary>
        /// Defines multiple mapppings between IContext (View Models) Types and Control (View) Types.
        /// </summary>
        /// <param name="mappings">An IDictionary linking IContext (View Model) Types and Control (View) Types.</param>
        public void SetContextControlMappings(IDictionary<Type, Type> mappings)
        {
            foreach (var kvp in mappings)
                SetContextControlMapping(kvp.Key, kvp.Value);
        }

        private bool CanRemoveTopContext()
            => _contexts.Count > 0;

        private void CloseContext(IContext context)
        {
            using (var d = new ContextServiceDeferrer(_contextServices[context]))
                RemoveContext(context);
        }

        private void RemoveContext(IContext context)
        {
            context.Dispose();
            _contexts.Remove(context);
            _contextServices[context].CloseCallback();
            _contextServices.Remove(context);
        }

        private void RemoveTopContext()
        {
            var context = _contexts[_contexts.Count - 1];
            context.SignalToClose(() => RemoveContext(context));
        }
    }
}
