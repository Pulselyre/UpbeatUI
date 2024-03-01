/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.Hosting
{
    public static class Extensions
    {
        /// <summary>
        /// Configures an <see cref="IHostBuilder"/> to create and start a <see cref="IHostedUpbeatService"/> to manage the <see cref="UpbeatStack"/> and main <see cref="Window"/>.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to configure UpbeatUI on.</param>
        /// <param name="baseViewModelParametersCreator">The delegate that will create parameters for the base/bottom ViewModel</param>
        /// <param name="configure">The delegate for configuring the <see cref="IHostedUpbeatBuilder"/> that will be used when starting the <see cref="IHostedUpbeatService"/>.</param>
        /// <returns>The <see cref="IHostBuilder"/> for chaining.</returns>
        public static IHostBuilder ConfigureUpbeatHost(
            this IHostBuilder hostBuilder, Func<object> baseViewModelParametersCreator,
            Action<IHostedUpbeatBuilder> configure = null) =>
            hostBuilder?.ConfigureServices(
                (hostBuilderContext, serviceCollection) => serviceCollection
                    .AddSingleton<IUpbeatApplicationService, UpbeatApplicationService>()
                    .AddHostedService(
                        sp =>
                        {
                            var upbeatHostBuilder = new HostedUpbeatBuilder();
                            _ = upbeatHostBuilder.ConfigureBaseViewModelParameters(baseViewModelParametersCreator)
                                .SetDefaultViewModelLocators();
                            configure?.Invoke(upbeatHostBuilder);
                            return new HostedUpbeatService(
                                upbeatHostBuilder,
                                sp,
                                sp.GetRequiredService<IHostApplicationLifetime>(),
                                sp.GetRequiredService<IUpbeatApplicationService>() as UpbeatApplicationService);
                        })) ?? throw new ArgumentNullException(nameof(hostBuilder));
    }
}
