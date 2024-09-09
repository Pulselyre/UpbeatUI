/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using System.Windows.Media;
using ManualUpbeatUISample.ViewModel;
using UpbeatUI.ViewModel;
using UpbeatUI.View;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace ManualUpbeatUISample;

public partial class App : Application
{
    // This app would like to provide a way for the user to cancel exiting. To do so, we create a shared task that can be triggered and reset.
    private TaskCompletionSource _closeRequestedTask = new();
    private Exception _exception;

    private async void HandleApplicationStartup(object sender, StartupEventArgs e)
    {
        using var sharedTimer = new SharedTimer();
        var overlayService = new OverlayService();

        // The UpbeatStack is the central data structure for an UpbeatUI app. One must be created for the life of the application and should be disposed at the end.
        using (var upbeatStack = new UpbeatStack())
        {

            // The UpbeatStack depends on mappings of Parameters types to ViewModels and controls to determine which ViewModel to create and which View to show. Without an IServiceProvider, you must manually map each Parameters, ViewModel, and View type, along with a constructur the IUpbeatStack can call to create a ViewModel.
            upbeatStack.MapViewModel<BottomViewModel.Parameters, BottomViewModel>(
                (service, parameters) => new BottomViewModel(service, sharedTimer));
            upbeatStack.MapViewModel<ConfirmPopupViewModel.Parameters, ConfirmPopupViewModel>(
                (upbeatService, parameters) => new ConfirmPopupViewModel(upbeatService, parameters, sharedTimer));

            // The MenuViewModel's constructor requires an Action that it can use to start closing the application. We will provide the shared _closeRequestedTask's TrySetResult() method to indicate the user has requested to close the application.
            upbeatStack.MapViewModel<MenuViewModel.Parameters, MenuViewModel>(
                (upbeatService, parameters) => new MenuViewModel(upbeatService, () => _closeRequestedTask.TrySetResult(), sharedTimer, overlayService));
            upbeatStack.MapViewModel<PopupViewModel.Parameters, PopupViewModel>(
                (upbeatService, parameters) => new PopupViewModel(parameters, sharedTimer));
            upbeatStack.MapViewModel<RandomDataViewModel.Parameters, RandomDataViewModel>(
                (upbeatService, parameters) => new RandomDataViewModel(upbeatService, RandomNumberGenerator.Create(), sharedTimer));
            upbeatStack.MapViewModel<SharedListViewModel.Parameters, SharedListViewModel>(
                (upbeatService, parameters) =>
                {
                    // The SharedListViewModel shares an IUpbeatService and scoped SharedList service with its child ViewModel, the SharedListDataViewModel. The scoped service can be manually created and provided to both.
                    var sharedList = new SharedList();
                    return new SharedListViewModel(upbeatService, sharedList, sharedTimer,
                        new SharedListDataViewModel(upbeatService, sharedList));
                });
            upbeatStack.MapViewModel<TextEntryPopupViewModel.Parameters, TextEntryPopupViewModel>(
                (upbeatService, parameters) => new TextEntryPopupViewModel(upbeatService, parameters, sharedTimer));

            using var overlayViewModel = new OverlayViewModel(overlayService);
            // The included UpdateMainWindow class already provides the necessary controls to display Views for ViewModels when an IUpbeatStack is set as the DataContext.
            var mainWindow = new UpbeatMainWindow()
            {
                // You must set the DataContext to the UpbeatStack.
                DataContext = upbeatStack,
                Title = "UpbeatUI Sample Application",
                MinHeight = 275,
                MinWidth = 275,
                Height = 400,
                Width = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ModalBackground = new SolidColorBrush(Brushes.LightGray.Color) { Opacity = 0.5 }, // The brush to display underneath the top View.
                ModalBlurEffect = new BlurEffect() { Radius = 10.0, KernelType = KernelType.Gaussian }, // The blur effect to apply to Views that are not on top. This is optional, as blur effects can significantly impact performance.
                OverlayDataContext = overlayViewModel,
            };

            // Override the default Window Closing event to request a close instead of immediately closing itself.
            void HandleMainWindowClosing(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                _ = _closeRequestedTask.TrySetResult();
            }
            mainWindow.Closing += HandleMainWindowClosing;
            // When the UpbeatStack has no more view models, the 'ViewModelsEmptied' event will be triggered, so we can count that as a request to close the application.
            void HandleUpbeatStackEmpied(object sender, EventArgs e) => _closeRequestedTask.TrySetResult();
            // Add a mechanism to catch unhandled exceptions.
            upbeatStack.ViewModelsEmptied += HandleUpbeatStackEmpied;
            void HandleApplicationException(object sender, DispatcherUnhandledExceptionEventArgs e)
            {
                e.Handled = true;
                _exception = e.Exception;
                _ = _closeRequestedTask.TrySetException(e.Exception);
            }
            Current.DispatcherUnhandledException += HandleApplicationException;

            try
            {
                // Add a base BottomViewModel to the UpbeatStack.
                upbeatStack.OpenViewModel(new BottomViewModel.Parameters());

                // We are now ready to show the main window.
                mainWindow.Show();

                // We set up an infinite loop to await the shared _closeRequestedTask, then attempt to close all ViewModels. If successful, we can exit the application. If not successful, possibly because the user cancelled closing, then reset the shared task and await again.
                while (true)
                {
                    await _closeRequestedTask.Task.ConfigureAwait(true);
                    if (await upbeatStack.TryCloseAllViewModelsAsync().ConfigureAwait(true))
                    {
                        break;
                    }
                    _closeRequestedTask = new TaskCompletionSource();
                }
            }
            catch (Exception ex)
            {
                _exception ??= ex;
            }
            finally
            {
                Current.DispatcherUnhandledException -= HandleApplicationException;
                upbeatStack.ViewModelsEmptied -= HandleUpbeatStackEmpied;
                mainWindow.Closing -= HandleMainWindowClosing;
                mainWindow.Close();
            }
        }
        if (_exception is not null)
        {
            if (MessageBox.Show(
                $"Error message: {_exception.Message}. See stack trace?",
                "Fatal Error",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                _ = MessageBox.Show(
                    _exception.StackTrace,
                    "Fatal Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.None);
            }
        }
    }
}

