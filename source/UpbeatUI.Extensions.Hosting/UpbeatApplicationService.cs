/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using UpbeatUI.Extensions.DependencyInjection;

namespace UpbeatUI.Extensions.Hosting
{
    internal class UpbeatApplicationService : IUpbeatApplicationService, IDisposable
    {
        protected readonly HostedUpbeatBuilder _upbeatHostBuilder;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ServiceProvidedUpbeatStack _upbeatStack;
        protected readonly IHostApplicationLifetime _hostApplicationLifetime;
        protected Task _applicationTask;

        protected UpbeatApplicationService(HostedUpbeatBuilder upbeatHostBuilder, IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _upbeatHostBuilder = upbeatHostBuilder ?? throw new ArgumentNullException(nameof(upbeatHostBuilder));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            _upbeatStack = new ServiceProvidedUpbeatStack(_serviceProvider);
        }

        public async void CloseUpbeatApplication()
        {
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

        public void Dispose() =>
            _upbeatStack.Dispose();
    }
}
