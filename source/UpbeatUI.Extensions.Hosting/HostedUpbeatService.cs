/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;
using UpbeatUI.Extensions.DependencyInjection;

namespace UpbeatUI.Extensions.Hosting
{
    internal class HostedUpbeatService : UpbeatApplicationService, IHostedUpbeatService
    {
        internal HostedUpbeatService(HostedUpbeatBuilder upbeatHostBuilder, IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
            : base(upbeatHostBuilder, serviceProvider, hostApplicationLifetime)
        { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var thread = new Thread(() =>
            {
                var app = new Application();
                app.Startup += async (sender, e) =>
                {
                    Window mainWindow = null;
                    Task baseViewModelOpen = null;
                    _upbeatStack = new ServiceProvidedUpbeatStack(_serviceProvider ?? throw new ArgumentNullException(nameof(_serviceProvider)));
                    foreach (var registerer in _upbeatHostBuilder.MappingRegisterers ?? throw new InvalidOperationException($"No {nameof(_upbeatHostBuilder.MappingRegisterers)} provided."))
                        registerer.Invoke(_upbeatStack);
                    mainWindow = _upbeatHostBuilder.WindowCreator?.Invoke() ?? throw new InvalidOperationException($"No {nameof(_upbeatHostBuilder.WindowCreator)} provided.");
                    mainWindow.DataContext = _upbeatStack;
                    mainWindow.Closing += MainWindowClosingRequested;
                    _upbeatStack.ViewModelsEmptyCallback = () => _forcedClosedTaskSource.SetResult(true);
                    baseViewModelOpen = _upbeatStack.OpenViewModelAsync(_upbeatHostBuilder.BaseViewModelParametersCreator?.Invoke() ?? throw new InvalidOperationException($"No {nameof(_upbeatHostBuilder.BaseViewModelParametersCreator)} provided."));
                    mainWindow.Show();
                    await Task.WhenAny(baseViewModelOpen ?? Task.CompletedTask, _forcedClosedTaskSource.Task);
                    mainWindow.Closing -= MainWindowClosingRequested;
                    mainWindow.Close();
                    if (_forcedClosedTaskSource.Task.IsFaulted)
                        throw _forcedClosedTaskSource.Task.Exception.InnerException;
                    _upbeatStack.ViewModelsEmptyCallback = null;
                };
                try
                {
                    app.Run();
                    _applicationTaskSource.SetResult(true);
                }
                catch (Exception ex)
                {
                    _applicationTaskSource.SetException(ex.InnerException ?? ex);
                }
                 _hostApplicationLifetime.StopApplication();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return _applicationTaskSource.Task;
        }

        public Task StopAsync(CancellationToken cancellationToken) =>
            _applicationTaskSource.Task;

        private void MainWindowClosingRequested(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            CloseUpbeatApplication();
        }
    }
}
