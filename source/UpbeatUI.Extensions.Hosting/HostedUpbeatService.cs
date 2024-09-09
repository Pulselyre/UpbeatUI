/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Hosting;
using UpbeatUI.Extensions.DependencyInjection;
using UpbeatUI.View;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.Hosting
{
    internal sealed class HostedUpbeatService : IHostedService
    {

        private readonly HostedUpbeatBuilder _upbeatHostBuilder;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly UpbeatApplicationService _upbeatApplicationService;
        private readonly IServiceProvider _serviceProvider;
        private Task _executeTask;
        private TaskCompletionSource<bool> _closeRequestedTask = new TaskCompletionSource<bool>();
        private Window _mainWindow;
        private Exception _exception;

        internal HostedUpbeatService(
            HostedUpbeatBuilder upbeatHostBuilder,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime hostApplicationLifetime,
            UpbeatApplicationService upbeatApplicationService)
        {
            _upbeatHostBuilder = upbeatHostBuilder ?? throw new ArgumentNullException(nameof(upbeatHostBuilder));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            _upbeatApplicationService = upbeatApplicationService ?? throw new ArgumentNullException(nameof(upbeatApplicationService));
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executeTask = ExecuteAsync();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _ = _closeRequestedTask.TrySetResult(true);
            return _executeTask;
        }

        private async Task ExecuteAsync()
        {
            try
            {
                using (var upbeatStack = new ServiceProvidedUpbeatStack(_serviceProvider))
                {
                    _upbeatApplicationService.CloseRequested += HandleUpbeatApplicationServiceCloseRequested;
                    foreach (var registerer in _upbeatHostBuilder.MappingRegisterers ?? throw new InvalidOperationException($"No {nameof(_upbeatHostBuilder.MappingRegisterers)} provided."))
                    {
                        registerer.Invoke(upbeatStack);
                    }
                    var overlayViewModel = _upbeatHostBuilder.OverlayViewModelCreator?.Invoke(_serviceProvider);
                    _mainWindow = (_upbeatHostBuilder.WindowCreator ?? new Func<IServiceProvider, IUpbeatStack, object, Window>((sp, us, ovm) => new UpbeatMainWindow())).Invoke(_serviceProvider, upbeatStack, overlayViewModel);
                    _mainWindow.Closing += HandleMainWindowClosing;
                    upbeatStack.ViewModelsEmptied += HandleUpbeatStackViewModelsEmptied;
                    Application.Current.DispatcherUnhandledException += HandleApplicationException;
                    try
                    {
                        upbeatStack.OpenViewModel(_upbeatHostBuilder.BaseViewModelParametersCreator?.Invoke() ?? throw new InvalidOperationException($"No {nameof(_upbeatHostBuilder.BaseViewModelParametersCreator)} provided."));
                        _mainWindow.Show();
                        while (true)
                        {
                            _ = await _closeRequestedTask.Task.ConfigureAwait(true);
                            if (await upbeatStack.TryCloseAllViewModelsAsync().ConfigureAwait(true))
                            {
                                break;
                            }
                            _closeRequestedTask = new TaskCompletionSource<bool>();
                        }
                    }
                    catch (Exception e)
                    {
                        if (_upbeatHostBuilder.FatalErrorHandler != null)
                        {
                            _exception ??= e;
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        Application.Current.DispatcherUnhandledException -= HandleApplicationException;
                        upbeatStack.ViewModelsEmptied -= HandleUpbeatStackViewModelsEmptied;
                        _mainWindow.Closing -= HandleMainWindowClosing;
                        _mainWindow.Close();
                        _upbeatApplicationService.CloseRequested -= HandleUpbeatApplicationServiceCloseRequested;
                        (_mainWindow as IDisposable)?.Dispose();
                        (overlayViewModel as IDisposable)?.Dispose();
                    }
                }
                if (_exception != null)
                {
                    _upbeatHostBuilder.FatalErrorHandler(_serviceProvider, _exception);
                }
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        private void HandleMainWindowClosing(object sender, CancelEventArgs e)
        {
            _ = e ?? throw new ArgumentNullException(nameof(e));
            e.Cancel = true;
            _ = _closeRequestedTask.TrySetResult(true);
        }

        private void HandleUpbeatApplicationServiceCloseRequested(object sender, EventArgs e) =>
            _ = _closeRequestedTask.TrySetResult(true);

        private void HandleUpbeatStackViewModelsEmptied(object sender, EventArgs e) =>
            _ = _closeRequestedTask.TrySetResult(true);

        private void HandleApplicationException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (_upbeatHostBuilder.FatalErrorHandler != null)
            {
                _ = e ?? throw new ArgumentNullException(nameof(e));
                e.Handled = true;
                _ = _closeRequestedTask.TrySetException(e.Exception);
            }
        }
    }
}
