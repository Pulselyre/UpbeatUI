/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using System.Windows.Media;
using HostedUpbeatUISample.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UpbeatUI.Extensions.Hosting;
using UpbeatUI.View;

namespace HostedUpbeatUISample;

public class Program
{
    private static void Main(string[] args) =>
        Host.CreateDefaultBuilder(args) // Use the .NET IHostBuilder to manage the HostedUpbeatService
            .ConfigureServices((hostContext, serviceCollection) => serviceCollection // Services can be configured, just as in other Hosted projects like ASP.NET Core applications. The IUpbeatStack will inject them into ViewModels when appropriate. Scoped services are supported, and each ViewModel within the stack is a separate scope.
                .AddTransient<Random>()
                .AddSingleton<SharedTimer>()
                .AddScoped<SharedList>())
            .ConfigureUpbeatHost( // Use this extension method to add UpbeatUI to the IHostBuilder
                () => new BottomViewModel.Parameters(), // Provide a delegate to create the bottom ViewModel. This is required so that the IUpbeatStack has something to show.
                builder => builder
                    .SetDefaultViewModelLocators() // The IUpbeatStack depends on mappings of Parameter types to ViewModel and Control types to determine which ViewModel to create and which View to show. (This line is optional, since the IUpbeatStack will use the default mappings without any configuration, but there are additional extension methods to change or customize the mappings.)
                    .ConfigureWindow(() => new UpbeatMainWindow() // The included UpdateMainWindow class already provides the necessary controls to display Views for IViewModels. The HostedUpbeatService will set the Window's DataContext automatically.
                    {
                        Title = "UpbeatUI Sample Application",
                        MinHeight = 275,
                        MinWidth = 275,
                        Height = 400,
                        Width = 400,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        BlurColor = new SolidColorBrush(Brushes.LightGray.Color) { Opacity = 0.5 }, // The brush to display underneath the top View.
                    }))
            .Build()
            .Run(); // Upon running, the IHost will start the HostedUpbeatService, which will create an IUpbeatStack and show the UpbeatManWindow (as configured above).
}
