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
            private readonly Action<object, Action> _childViewModelOpener;
            private Func<Task<bool>> _asyncOkToCloseCallback;
            private Action _closer;
            private Action<Action> _deferrer;
            private Func<bool> _isActiveViewModel;
            private Action _updateCallback;

            internal UpbeatService(bool updatesOnRender, Action<object, Action> childViewModelOpener, Action closedCallback = null)
            {
                UpdatesOnRender = updatesOnRender;
                _childViewModelOpener = childViewModelOpener;
                ClosedCallback = closedCallback;
            }

            [Obsolete("Renamed to IsActiveViewModel. This property will be removed in UpbeatUI 3.0.")]
            public bool IsActiveUpbeatViewModel => IsActiveViewModel;
            public bool IsActiveViewModel => _isActiveViewModel();
            public bool UpdatesOnRender { get; }
            internal Action ClosedCallback { get; }

            public void Close()
            {
                if (_deferrer == null)
                    _closer();
                else
                    _deferrer(() => _closer());
            }

            [Obsolete("This will be removed in UpbeatUI 3.0. Use another injected service instead.")]
            public string GetClipboard() =>
                Clipboard.GetText();

            [Obsolete("Renamed to OpenViewModel. This method will be removed in UpbeatUI 3.0.")]
            public void OpenUpbeatViewModel<TParameters>(TParameters parameters) =>
                OpenViewModel(parameters);

            [Obsolete("Renamed to OpenViewModel. This method will be removed in UpbeatUI 3.0.")]
            public void OpenUpbeatViewModel<TParameters>(TParameters parameters, Action closedCallback) =>
                OpenViewModel(parameters, closedCallback);

            [Obsolete("Renamed to OpenViewModelAsync. This method will be removed in UpbeatUI 3.0.")]
            public Task OpenUpbeatViewModelAsync<TParameters>(TParameters parameters) =>
                OpenViewModelAsync(parameters);

            public void OpenViewModel<TParameters>(TParameters parameters) =>
                OpenViewModel(parameters, null);

            public void OpenViewModel<TParameters>(TParameters parameters, Action closedCallback)
            {
                if (_deferrer == null)
                    _childViewModelOpener(parameters, closedCallback);
                else
                    _deferrer(() => _childViewModelOpener(parameters, closedCallback));
            }

            public Task OpenViewModelAsync<TParameters>(TParameters parameters)
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                OpenViewModel(
                    parameters,
                    () => taskCompletionSource.SetResult(true));
                return taskCompletionSource.Task;
            }

            [Obsolete("This will be removed in UpbeatUI 3.0. Use another injected service instead.")]
            public void SetClipboard(string text) =>
                    Clipboard.SetText(text);

            public void SetCloseCallback(Func<bool> okToCloseCallback) =>
                _asyncOkToCloseCallback = () => Task.FromResult(okToCloseCallback());

            public void SetCloseCallback(Func<Task<bool>> asyncOkToCloseCallback) =>
                _asyncOkToCloseCallback = asyncOkToCloseCallback;

            public void SetUpdateCallback(Action updateCallback) =>
                _updateCallback = updateCallback;

            internal object Activate(Func<IUpbeatService, object> viewModelCreator, Func<object, bool> isActiveViewModel, Action<object> closer)
            {
                var viewModel = viewModelCreator(this);
                _isActiveViewModel = () => isActiveViewModel(viewModel);
                _closer = () => closer(viewModel);
                if (viewModel is IUpdatableViewModel updatableViewModel)
                    _updateCallback = updatableViewModel.UpdateViewModelProperties;
                if (viewModel is IUpbeatViewModel upbeatViewModel)
                    _asyncOkToCloseCallback = () =>
                    {
                        bool okToClose = false;
                        upbeatViewModel.SignalToClose(() => okToClose = true);
                        return Task.FromResult(okToClose);
                    };
                return viewModel;
            }

            internal void Lock(Action<Action> deferrer) =>
                _deferrer = deferrer;

            internal Task<bool> OkToCloseAsync() =>
                _asyncOkToCloseCallback?.Invoke() ?? Task.FromResult(true);

            internal void Unlock() =>
                _deferrer = null;

            internal void Update() =>
                _updateCallback?.Invoke();
        }
    }
}
