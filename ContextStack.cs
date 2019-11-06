using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UpbeatUI
{
    public class ContextStack : ObservableObject, IDisposable, IUpdatableContext
    {
        private ObservableCollection<IContext> _contexts = new ObservableCollection<IContext>();
        private Action _contextsEmptyCallback;
        private IDictionary<IContext, ContextService> _contextServices = new Dictionary<IContext, ContextService>();

        public ContextStack(Action contextsEmptyCallback)
        {
            _contextsEmptyCallback = contextsEmptyCallback;
            Contexts = new ReadOnlyObservableCollection<IContext>(_contexts);
            RemoveTopContextCommand = new ObservableCommand(RemoveTopContext, CanRemoveTopContext);
        }

        public INotifyCollectionChanged Contexts { get; }
        public ICommand RemoveTopContextCommand { get; }
        public int Count { get { return _contexts.Count; } }

        public void AddContext(ContextCreator creator)
            => AddContext(creator, null);

        public void AddContext(ContextCreator creator, Action closeCallback)
        {
            var contextService = new ContextService(AddContext, CloseContext, closeCallback);
            using (var d = new ContextServiceDeferrer(contextService))
            {
                var context = contextService.CreateContext(creator);
                _contextServices[context] = contextService;
                _contexts.Add(context);
            }
        }

        public void Dispose()
        {
            foreach (var context in _contexts.Reverse())
                context.Dispose();
        }

        public void UpdateContextProperties()
        {
            foreach (var context in _contexts
                .Where(c => typeof(IUpdatableContext).IsAssignableFrom(c.GetType()))
                .Select(c => (IUpdatableContext)c))
                context.UpdateContextProperties();
        }

        public void RemoveAllContexts()
        {
            if (_contexts.Count == 0)
                _contextsEmptyCallback();
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

        private class ContextService : IContextService
        {
            private Action<IContext> _closer;
            private Action _closedCallback;
            private IContext _context;
            private Action<ContextCreator, Action> _opener;
            private Action<Action> _deferrer;

            internal ContextService(Action<ContextCreator, Action> opener, Action<IContext> closer, Action closedCallback)
            {
                _opener = opener;
                _closer = closer;
                _closedCallback = closedCallback;
            }

            public void Close()
            {
                if (_deferrer == null)
                    _closer(_context);
                else
                    _deferrer(() => _closer(_context));
            }

            public string GetClipboard()
                => Clipboard.GetText();

            public void OpenContext(ContextCreator contextCreator)
                => OpenContext(contextCreator, null);

            public void OpenContext(ContextCreator contextCreator, Action closedCallback)
            {
                if (_deferrer == null)
                    _opener(contextCreator, closedCallback);
                else
                    _deferrer(() => _opener(contextCreator, closedCallback));
            }

            public async Task OpenContextAsync(ContextCreator contextCreator)
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                OpenContext(
                    contextCreator,
                    () => taskCompletionSource.SetResult(true));
                await taskCompletionSource.Task;
            }

            public void SetClipboard(string text)
                => Clipboard.SetText(text);

            internal void CloseCallback()
                => _closedCallback?.Invoke();

            internal IContext CreateContext(ContextCreator creator)
            {
                _context = creator(this);
                return _context;
            }

            internal void Lock(Action<Action> deferrer)
                => _deferrer = deferrer;

            internal void Unlock()
                => _deferrer = null;
        }

        private class ContextServiceDeferrer : ActionDeferrer
        {
            public ContextServiceDeferrer(ContextService configurationService)
                : base(configurationService.Lock, configurationService.Unlock)
            { }
        }
    }
}
