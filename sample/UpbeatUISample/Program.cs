/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using UpbeatUI.View;
using UpbeatUI.ViewModel;
using UpbeatUISample.View;
using UpbeatUISample.ViewModel;

namespace UpbeatUISample
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var app = new Application();
            app.Startup += async (sender, e) =>
            {
                // The UpbeatStack is the central data structure for an UpbeatUI app. One must be created for the life of the application and should be disposed at the end.
                using (var upbeatStack = new UpbeatStack())
                {
                    // The UpbeatStack depends on mappings of parameter types to IUpbeatViewModels and controls to determine which IUpbeatViewMOdel to create and which View to show.
                    upbeatStack.MapViewModel<BottomViewModel.Parameters, BottomViewModel, BottomControl>(
                        (service, parameters) => new BottomViewModel(service, parameters));
                    upbeatStack.MapViewModel<MenuViewModel.Parameters, MenuViewModel, MenuControl>(
                        (service, parameters) => new MenuViewModel(service, parameters));
                    upbeatStack.MapViewModel<PopupViewModel.Parameters, PopupViewModel, PopupControl>(
                        (service, parameters) => new PopupViewModel(service, parameters));
                    upbeatStack.MapViewModel<PositionedPopupViewModel.Parameters, PositionedPopupViewModel, PositionedPopupControl>(
                        (service, parameters) => new PositionedPopupViewModel(service, parameters));
                    upbeatStack.MapViewModel<ScaledPopupViewModel.Parameters, ScaledPopupViewModel, ScaledPopupControl>(
                        (service, parameters) => new ScaledPopupViewModel(service, parameters));
                    upbeatStack.MapViewModel<ConfirmPopupViewModel.Parameters, ConfirmPopupViewModel, ConfirmPopupControl>(
                        (service, parameters) => new ConfirmPopupViewModel(service, parameters));

                    // The included UpdateMainWindow class already provides the necessary controls to display Views for IUpbeatViewModels in a UpbeatStack set as the DataContext.
                    var mainWindow = new UpbeatMainWindow()
                    {
                        DataContext = upbeatStack,
                        Title = "UpbeatUI Sample Application",
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        BlurColor = new SolidColorBrush(Brushes.OrangeRed.Color) { Opacity = 0.5 }, // The brush to overlay Views underneath the top View.
                    };

                    // Override the default Window Closing event to ensure that the UpbeatStack and all of its children IUpbeatViewModels are properly disposed.
                    CancelEventHandler closingHandler =
                        async (sender, e) =>
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
                        };
                    mainWindow.Closing += closingHandler;
                    mainWindow.Show();
                    // Add a base BottomViewModel to the UpbeatStack and wait for it to be closed.
                    await upbeatStack.OpenViewModelAsync(new BottomViewModel.Parameters(upbeatStack.TryCloseAllViewModelsAsync));

                    mainWindow.Closing -= closingHandler;
                    mainWindow.Close();
                }
            };
            app.Run();
        }
    }
}
