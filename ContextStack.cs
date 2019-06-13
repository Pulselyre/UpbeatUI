using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
        {
            var contextService = new ContextService(AddContext, CloseContext);
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
            private IContext _context;
            private Action<ContextCreator> _opener;
            private Action<Action> _deferrer;

            internal ContextService(Action<ContextCreator> opener, Action<IContext> closer)
            {
                _opener = opener;
                _closer = closer;
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
            {
                if (_deferrer == null)
                    _opener(contextCreator);
                else
                    _deferrer(() => _opener(contextCreator));
            }

            public void SetClipboard(string text)
                => Clipboard.SetText(text);

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
