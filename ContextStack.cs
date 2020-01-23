using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UpbeatUI
{
    public partial class ContextStack : ObservableObject, IDisposable, IUpdatableContext
    {
        private ObservableCollection<IContext> _contexts = new ObservableCollection<IContext>();
        private Action _contextsEmptyCallback;
        private IDictionary<IContext, ContextService> _contextServices = new Dictionary<IContext, ContextService>();

        public ContextStack()
            : this(null) { }

        public ContextStack(Action contextsEmptyCallback)
        {
            _contextsEmptyCallback = contextsEmptyCallback;
            Contexts = new ReadOnlyObservableCollection<IContext>(_contexts);
            RemoveTopContextCommand = new ObservableCommand(RemoveTopContext, CanRemoveTopContext);
        }

        public INotifyCollectionChanged Contexts { get; }
        public ICommand RemoveTopContextCommand { get; }
        public int Count { get { return _contexts.Count; } }

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
                _contextsEmptyCallback?.Invoke();
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
