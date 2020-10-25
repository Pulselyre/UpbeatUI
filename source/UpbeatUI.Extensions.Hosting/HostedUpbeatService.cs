/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;

namespace UpbeatUI.Extensions.Hosting
{
    internal class HostedUpbeatService : UpbeatApplicationService, IHostedUpbeatService
    {
        internal HostedUpbeatService(HostedUpbeatBuilder upbeatHostBuilder, IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
            : base(upbeatHostBuilder, serviceProvider, hostApplicationLifetime)
        { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var startupTcs = new TaskCompletionSource<bool>();
            var applicationTcs = new TaskCompletionSource<bool>();
            _applicationTask = applicationTcs.Task;
            var thread = new Thread(() =>
            {
                var app = new Application();
                app.Startup += async (sender, e) =>
                {
                    Window mainWindow = null;
                    Task baseViewModelOpen = null;
                    try
                    {
                        foreach (var registerer in _upbeatHostBuilder.MappingRegisterers ?? throw new InvalidOperationException($"No {nameof(_upbeatHostBuilder.MappingRegisterers)} provided."))
                            registerer.Invoke(_upbeatStack, _serviceProvider);
                        mainWindow = _upbeatHostBuilder.WindowCreator?.Invoke() ?? throw new InvalidOperationException($"No {nameof(_upbeatHostBuilder.WindowCreator)} provided.");
                        mainWindow.DataContext = _upbeatStack;
                        baseViewModelOpen = _upbeatStack.OpenViewModelAsync(_upbeatHostBuilder.BaseViewModelParametersCreator?.Invoke() ?? throw new InvalidOperationException($"No {nameof(_upbeatHostBuilder.BaseViewModelParametersCreator)} provided."));
                        mainWindow.Closing += MainWindowClosingRequested;
                        mainWindow.Show();
                        startupTcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        startupTcs.TrySetException(ex);
                        _hostApplicationLifetime.StopApplication();
                    }
                    try
                    {
                        await (baseViewModelOpen ?? Task.CompletedTask);
                        if (mainWindow != null)
                        {
                            mainWindow.Closing -= MainWindowClosingRequested;
                            mainWindow.Close();
                        }
                        applicationTcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        applicationTcs.TrySetException(ex);
                        _hostApplicationLifetime.StopApplication();
                    }
                };
                app.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return startupTcs.Task;
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            _applicationTask;

        private async void MainWindowClosingRequested(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            try
            {
                if (await _upbeatStack.TryCloseAllViewModelsAsync())
                    _hostApplicationLifetime.StopApplication();
            }
            catch (Exception)
            {
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
