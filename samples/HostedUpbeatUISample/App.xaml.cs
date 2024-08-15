/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using HostedUpbeatUISample.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UpbeatUI.Extensions.Hosting;
using UpbeatUI.View;

namespace HostedUpbeatUISample;

public partial class App : Application
{
    private async void HandleApplicationStartup(object sender, StartupEventArgs e) =>
        await Host.CreateDefaultBuilder(e?.Args ?? Array.Empty<string>()) // Use the .NET IHostBuilder to manage the HostedUpbeatService
            .ConfigureServices((hostContext, serviceCollection) => serviceCollection // Services can be configured, just as in other Hosted projects like ASP.NET Core applications. The IUpbeatStack will inject them into ViewModels when appropriate. Scoped services are supported, and each ViewModel within the stack is a separate scope.
                .AddTransient(sp => RandomNumberGenerator.Create())
                .AddSingleton<SharedTimer>()
                .AddScoped<SharedList>())
            .ConfigureUpbeatHost( // Use this extension method to add UpbeatUI to the IHostBuilder
                () => new BottomViewModel.Parameters(), // Provide a delegate to create the bottom ViewModel. This is required so that the IUpbeatStack has something to show.
                builder => builder
                    .SetDefaultViewModelLocators() // The IUpbeatStack depends on mappings of Parameters types to ViewModel and Control types to determine which ViewModel to create and which View to show. (This line is optional, since the IUpbeatStack will use the default mappings without any configuration, but there are additional extension methods to change or customize the mappings.)
                    .ConfigureWindow(() => new UpbeatMainWindow() // The included UpdateMainWindow class already provides the necessary controls to display Views for ViewModels. The HostedUpbeatService will set the Window's DataContext automatically.
                    {
                        Title = "UpbeatUI Sample Application",
                        MinHeight = 275,
                        MinWidth = 275,
                        Height = 400,
                        Width = 400,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        ModalBackground = new SolidColorBrush(Brushes.LightGray.Color) { Opacity = 0.5 }, // The brush to display underneath the top View.
                        ModalBlurEffect = new BlurEffect() { Radius = 10.0, KernelType = KernelType.Gaussian }, // The blur effect to apply to Views that are not on top. This is optional, as blur effects can significantly impact performance.
                    })
                    .SetFatalErrorHandler(e => MessageBox.Show($"Exception: {e.GetType().FullName} {e.Message}")))
            .Build()
            .RunAsync().ConfigureAwait(true);
}

