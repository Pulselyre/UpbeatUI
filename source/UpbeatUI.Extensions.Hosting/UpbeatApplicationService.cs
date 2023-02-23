/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
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
        protected readonly IHostApplicationLifetime _hostApplicationLifetime;
        protected readonly TaskCompletionSource<bool> _applicationTaskSource = new TaskCompletionSource<bool>();
        protected readonly TaskCompletionSource<bool> _forcedClosedTaskSource = new TaskCompletionSource<bool>();
        protected readonly IServiceProvider _serviceProvider;
        protected ServiceProvidedUpbeatStack _upbeatStack;

        protected UpbeatApplicationService(HostedUpbeatBuilder upbeatHostBuilder, IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _upbeatHostBuilder = upbeatHostBuilder ?? throw new ArgumentNullException(nameof(upbeatHostBuilder));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            _serviceProvider = serviceProvider;
        }

        public async void CloseUpbeatApplication()
        {
            try
            {
                await _upbeatStack.TryCloseAllViewModelsAsync();
            }
            catch (Exception e)
            {
                _forcedClosedTaskSource.SetException(e);
            }
        }

        public void Dispose() =>
            _upbeatStack.Dispose();
    }
}
