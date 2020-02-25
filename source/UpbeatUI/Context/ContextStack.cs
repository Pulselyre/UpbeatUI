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
    public partial class ContextStack : ObservableObject, IDisposable, IUpdatableContext
    {
        private IDictionary<Type, Type> _contextControlMappings = new Dictionary<Type, Type>();
        private ObservableCollection<IContext> _contexts = new ObservableCollection<IContext>();
        private IDictionary<IContext, ContextService> _contextServices = new Dictionary<IContext, ContextService>();

        public ContextStack()
        {
            ContextControlMappings = new ReadOnlyDictionary<Type, Type>(_contextControlMappings);
            Contexts = new ReadOnlyObservableCollection<IContext>(_contexts);
            RemoveTopContextCommand = new ObservableCommand(RemoveTopContext, CanRemoveTopContext);
        }

        public IReadOnlyDictionary<Type, Type> ContextControlMappings { get; }
        public INotifyCollectionChanged Contexts { get; }
        public int Count { get { return _contexts.Count; } }
        public ICommand RemoveTopContextCommand { get; }
        public Action ContextsEmptyCallback { get; set; }

        public void Dispose()
        {
            foreach (var context in _contexts.Reverse())
                context.Dispose();
        }

        public void OpenContext(ContextCreator creator)
            => OpenContext(creator, null);

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

        public async Task OpenContextAsync(ContextCreator contextCreator)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            OpenContext(
                contextCreator,
                () => taskCompletionSource.SetResult(true));
            await taskCompletionSource.Task;
        }

        public void UpdateContextProperties()
        {
            foreach (var context in _contexts)
                (context as IUpdatableContext)?.UpdateContextProperties();
        }

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

        public void RenderingHandler(object sender, EventArgs e) =>
            UpdateContextProperties();

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
