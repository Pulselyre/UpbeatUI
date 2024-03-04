/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using ServiceProvidedUpbeatUISample.ViewModel;
using UpbeatUI.View;
using UpbeatUI.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ServiceProvidedUpbeatUISample;

public partial class App : Application
{
    // This app would like to provide a way for the user to cancel exiting. To do so, we create a shared task that can be triggered and reset..
    private TaskCompletionSource _closeRequestedTask = new();

    private async void HandleApplicationStartup(object sender, StartupEventArgs e)
    {
        // Use a ServiceProvider (built from a ServiceCollection) to set up dependencies that the ServiceProvidedUpbeatStack will inject into ViewModels. Scoped services are supported, and each ViewModel within the stack is a separate scope.
        using var serviceProvider = new ServiceCollection()
            .AddTransient(sp => RandomNumberGenerator.Create())
            .AddSingleton<SharedTimer>()
            .AddScoped<SharedList>()
            .BuildServiceProvider();

        // The ServiceProvidedUpbeatStack is the central data structure for an UpbeatUI app. One must be created for the life of the application and should be disposed at the end. Unlike the basic UpbeatStack, the ServiceProvidedUpbeatStack requires an IServiceProvider to resolve dependencies for ViewModels.
        using var upbeatStack = new ServiceProvidedUpbeatStack(serviceProvider);

        // Instead of manually mapping parameter types to ViewModels and controls, the ServiceProvidedUpbeatStack can automatically map types based on naming convention. Use this method to enable the default naming convention, but other methods enable you to use your own naming conventions.
        upbeatStack.SetDefaultViewModelLocators();

        // The MenuViewModel requires a manual mapping, as its constructor requires an Action that it can use to request closing the application. We can use the shared TaskCompletionSource '_closeRequested', but that cannot be resolved by the IServiceProvider without creating additional service classes.
        upbeatStack.MapViewModel<MenuViewModel.Parameters, MenuViewModel>(
            (upbeatService, parameters, serviceProvider) => new MenuViewModel(
                    upbeatService,
                    () => _closeRequestedTask.TrySetResult(),
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

        // Override the default Window Closing event to request a close instead of closing itself.
        void HandleMainWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _ = _closeRequestedTask.TrySetResult();
        }
        mainWindow.Closing += HandleMainWindowClosing;
        // When the UpbeatStack has no more view models, the 'ViewModelsEmpited' event will be triggered, so we can count that as a request to close the application.
        void HandleUpbeatStackEmpied(object sender, EventArgs e) => _closeRequestedTask.TrySetResult();
        upbeatStack.ViewModelsEmptied += HandleUpbeatStackEmpied;

        // Add a base BottomViewModel to the UpbeatStack and wait for it to be closed.
        upbeatStack.OpenViewModel(new BottomViewModel.Parameters());

        // We are now ready to show the main window.
        mainWindow.Show();

        // We set up an infinite loop to await the shared close requested task, then attempt to close all ViewModels. If successful, we can exit the application. If not, reset the shared task and await again.
        while (true)
        {
            await _closeRequestedTask.Task.ConfigureAwait(true);
            if (await upbeatStack.TryCloseAllViewModelsAsync().ConfigureAwait(true))
            {
                break;
            }
            _closeRequestedTask = new TaskCompletionSource();
        }

        upbeatStack.ViewModelsEmptied -= HandleUpbeatStackEmpied;
        mainWindow.Closing -= HandleMainWindowClosing;
        mainWindow.Close();
    }
}

