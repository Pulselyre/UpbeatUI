/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows;

namespace UpbeatUI.Context
{
    public partial class ContextStack : ObservableObject, IDisposable
    {
        private class ContextService : IContextService
        {
            private Action<IContext> _closer;
            private Action _closedCallback;
            private Predicate<IContext> _isActiveContext;
            private IContext _context;
            private Action<ContextCreator, Action> _opener;
            private Action<Action> _deferrer;

            internal ContextService(Action<ContextCreator, Action> opener, Action<IContext> closer, Action closedCallback, Predicate<IContext> isActiveContext)
            {
                _opener = opener;
                _closer = closer;
                _closedCallback = closedCallback;
                _isActiveContext = isActiveContext;
            }

            public bool IsActiveContext => _isActiveContext(_context);

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
    }
}
