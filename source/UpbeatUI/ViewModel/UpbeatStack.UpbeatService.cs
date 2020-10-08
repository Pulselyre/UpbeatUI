/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using System.Windows;

namespace UpbeatUI.ViewModel
{
    public partial class UpbeatStack : BaseViewModel, IDisposable
    {
        private class UpbeatService : IUpbeatService
        {
            private Action<IUpbeatViewModel> _closer;
            private Action _closedCallback;
            private Predicate<IUpbeatViewModel> _isActiveUpbeatViewModel;
            private IUpbeatViewModel _upbeatViewModel;
            private Action<UpbeatViewModelCreator, Action> _opener;
            private Action<Action> _deferrer;

            internal UpbeatService(Action<UpbeatViewModelCreator, Action> opener, Action<IUpbeatViewModel> closer, Action closedCallback, Predicate<IUpbeatViewModel> isActiveUpbeatViewModel)
            {
                _opener = opener;
                _closer = closer;
                _closedCallback = closedCallback;
                _isActiveUpbeatViewModel = isActiveUpbeatViewModel;
            }

            public bool IsActiveUpbeatViewModel => _isActiveUpbeatViewModel(_upbeatViewModel);

            public void Close()
            {
                if (_deferrer == null)
                    _closer(_upbeatViewModel);
                else
                    _deferrer(() => _closer(_upbeatViewModel));
            }

            public string GetClipboard()
                => Clipboard.GetText();

            public void OpenUpbeatViewModel(UpbeatViewModelCreator upbeatViewModelCreator)
                => OpenUpbeatViewModel(upbeatViewModelCreator, null);

            public void OpenUpbeatViewModel(UpbeatViewModelCreator upbeatViewModelCreator, Action closedCallback)
            {
                if (_deferrer == null)
                    _opener(upbeatViewModelCreator, closedCallback);
                else
                    _deferrer(() => _opener(upbeatViewModelCreator, closedCallback));
            }

            public async Task OpenUpbeatViewModelAsync(UpbeatViewModelCreator upbeatViewModelCreator)
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                OpenUpbeatViewModel(
                    upbeatViewModelCreator,
                    () => taskCompletionSource.SetResult(true));
                await taskCompletionSource.Task;
            }

            public void SetClipboard(string text)
                => Clipboard.SetText(text);

            internal void CloseCallback()
                => _closedCallback?.Invoke();

            internal IUpbeatViewModel CreateUpbeatViewModel(UpbeatViewModelCreator upbeatViewModelCreator)
            {
                _upbeatViewModel = upbeatViewModelCreator(this);
                return _upbeatViewModel;
            }

            internal void Lock(Action<Action> deferrer)
                => _deferrer = deferrer;

            internal void Unlock()
                => _deferrer = null;
        }
    }
}
