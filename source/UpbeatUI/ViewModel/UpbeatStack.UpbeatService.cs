/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UpbeatUI.ViewModel
{
    public partial class UpbeatStack : BaseViewModel, IDisposable
    {
        private class UpbeatService : IUpbeatService
        {
            private readonly Action<object, Action> _childViewModelOpener;
            private List<Func<Task<bool>>> _asyncOkToCloseCallbacks = new List<Func<Task<bool>>>();
            private Action _closer;
            private Action<Action> _deferrer;
            private Func<bool> _isActiveViewModel;
            private List<Action> _updateCallbacks = new List<Action>();

            internal UpbeatService(bool updatesOnRender, Action<object, Action> childViewModelOpener, Action closedCallback = null)
            {
                UpdatesOnRender = updatesOnRender;
                _childViewModelOpener = childViewModelOpener;
                ClosedCallback = closedCallback;
            }

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

            public void RegisterCloseCallback(Func<bool> okToCloseCallback) =>
                _asyncOkToCloseCallbacks.Add(() => Task.FromResult(okToCloseCallback()));

            public void RegisterCloseCallback(Func<Task<bool>> asyncOkToCloseCallback) =>
                _asyncOkToCloseCallbacks.Add(asyncOkToCloseCallback);

            public void RegisterUpdateCallback(Action updateCallback) =>
                _updateCallbacks.Add(updateCallback);

            [Obsolete("Method has been renamed to 'RegisterCloseCallback' which better describes how the UpbeatStack handles multiple okToCloseCallback. The 'SetCloseCallback' method will be removed in UpbeatUI 5.0.0.")]
            public void SetCloseCallback(Func<bool> okToCloseCallback) =>
                RegisterCloseCallback(okToCloseCallback);

            [Obsolete("Method has been renamed to 'RegisterCloseCallback' which better describes how the UpbeatStack handles multiple asyncOkToCloseCallback. The 'SetCloseCallback' method will be removed in UpbeatUI 5.0.0.")]
            public void SetCloseCallback(Func<Task<bool>> asyncOkToCloseCallback) =>
                RegisterCloseCallback(asyncOkToCloseCallback);

            [Obsolete("Method has been renamed to 'RegisterUpdateCallback' which better describes how the UpbeatStack handles multiple updateCallbacks. The 'SetUpdateCallback' method will be removed in UpbeatUI 5.0.0.")]
            public void SetUpdateCallback(Action updateCallback) =>
                RegisterUpdateCallback(updateCallback);

            internal object Activate(Func<IUpbeatService, object> viewModelCreator, Func<object, bool> isActiveViewModel, Action<object> closer)
            {
                var viewModel = viewModelCreator(this);
                _isActiveViewModel = () => isActiveViewModel(viewModel);
                _closer = () => closer(viewModel);
                return viewModel;
            }

            internal void Lock(Action<Action> deferrer) =>
                _deferrer = deferrer;

            internal async Task<bool> OkToCloseAsync()
            {
                foreach (var asyncOkToCloseCallback in _asyncOkToCloseCallbacks)
                    if (!await asyncOkToCloseCallback())
                        return false;
                return true;
            }

            internal void Unlock() =>
                _deferrer = null;

            internal void Update()
            {
                foreach (var updateCallback in _updateCallbacks)
                    updateCallback.Invoke();
            }
        }
    }
}
