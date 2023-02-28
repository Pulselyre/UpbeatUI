/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using UpbeatUI.Extensions.DependencyInjection;
using UpbeatUI.View;
using ServiceProvidedUpbeatUISample.ViewModel;
using ServiceProvidedUpbeatUISample.View;

namespace ServiceProvidedUpbeatUISample;

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var app = new Application();
        app.Startup += async (sender, e) =>
        {
            // Use a ServiceCollection to set up dependencies that the ServiceProvidedUpbeatStack will inject into ViewModels. Scoped services are supported, and each ViewModel within the stack is a separate scope.
            var serviceCollection = new ServiceCollection()
                .AddTransient<Random>()
                .AddSingleton<SharedTimer>()
                .AddScoped<SharedList>();

            // The ServiceProvidedUpbeatStack is the central data structure for an UpbeatUI app. One must be created for the life of the application and should be disposed at the end. Unlike the basic UpbeatStack, the ServiceProvidedUpbeatStack requires an IServiceProvider to resolve dependencies for ViewModels.
            using var upbeatStack = new ServiceProvidedUpbeatStack(serviceCollection.BuildServiceProvider());

            // Instead of manually mapping parameter types to ViewModels and controls, the ServiceProvidedUpbeatStack can automatically map types based on naming convention. Use this method to enable the default naming convention, but other methods enable you to use your own naming conventions.
            upbeatStack.SetDefaultViewModelLocators();

            // The MenuViewModel requires a manual mapping, as its constructor requires an async delegate that it can use to start closing the application. The IUpbeatStack includes an appropriate method: TryCloseAllViewModelsAsync, but it cannot be resolved by the IServiceProvider without creating additional service classes.
            upbeatStack.MapViewModel<MenuViewModel.Parameters, MenuViewModel, MenuControl>(
                (upbeatService, parameters, serviceProvider) => new MenuViewModel(
                        upbeatService,
                        upbeatStack.TryCloseAllViewModelsAsync,
                        serviceProvider.GetRequiredService<SharedTimer>()));

            // The included UpdateMainWindow class already provides the necessary controls to display Views for IUpbeatViewModels in a UpbeatStack set as the DataContext.
            var mainWindow = new UpbeatMainWindow()
            {
                // You must set the DataContext to the IUpbeatStack for the ViewModels to be used.
                DataContext = upbeatStack,
                Title = "UpbeatUI Sample Application",
                MinHeight = 275,
                MinWidth = 275,
                Height = 400,
                Width = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                BlurColor = new SolidColorBrush(Brushes.LightGray.Color) { Opacity = 0.5 }, // The brush to display underneath the top View.
            };

            // Override the default Window Closing event to ensure that the UpbeatStack and all of its children IUpbeatViewModels are properly disposed.
            async void ClosingHandler(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                try
                {
                    // Give currently open ViewModels a chance to cancel. If successful, the base/bottom ViewModel (opened below) will be closed and the awaited task will end.
                    await upbeatStack.TryCloseAllViewModelsAsync();
                    throw new Exception();
                }
                catch (Exception)
                {
                    // Something went wrong in attempting to close
                    upbeatStack.Dispose();
                }
            }
            mainWindow.Closing += ClosingHandler;
            mainWindow.Show();

            // Add a base BottomViewModel to the UpbeatStack and wait for it to be closed.
            await upbeatStack.OpenViewModelAsync(new BottomViewModel.Parameters());

            mainWindow.Closing -= ClosingHandler;
            mainWindow.Close();
        };
        app.Run();
    }
}
