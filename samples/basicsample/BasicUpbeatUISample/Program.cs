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
using BasicUpbeatUISample.View;
using BasicUpbeatUISample.ViewModel;

namespace BasicUpbeatUISample
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var app = new Application();
            app.Startup += async (sender, e) =>
            {
                using (var sharedTimer = new SharedTimer())
                // The UpbeatStack is the central data structure for an UpbeatUI app. One must be created for the life of the application and should be disposed at the end.
                using (var upbeatStack = new UpbeatStack())
                {
                    // The UpbeatStack depends on mappings of parameter types to ViewModels and controls to determine which ViewModel to create and which View to show. Without an IServiceProvider, you must manually map each Parameters, ViewModel, and View type, along with a constructur the IUpbeatStack can call to create a ViewModel.
                    upbeatStack.MapViewModel<BottomViewModel.Parameters, BottomViewModel, BottomControl>(
                        (service, parameters) => new BottomViewModel(service, sharedTimer));
                    upbeatStack.MapViewModel<ConfirmPopupViewModel.Parameters, ConfirmPopupViewModel, ConfirmPopupControl>(
                        (upbeatService, parameters) => new ConfirmPopupViewModel(upbeatService, parameters, sharedTimer));
                    upbeatStack.MapViewModel<MenuViewModel.Parameters, MenuViewModel, MenuControl>(
                        // The MenuViewModel's constructor requires an async delegate that it can use to start closing the application. The IUpbeatStack includes an appropriate method: TryCloseAllViewModelsAsync.
                        (upbeatService, parameters) => new MenuViewModel(upbeatService, upbeatStack.TryCloseAllViewModelsAsync, sharedTimer));
                    upbeatStack.MapViewModel<PopupViewModel.Parameters, PopupViewModel, PopupControl>(
                        (upbeatService, parameters) => new PopupViewModel(parameters, sharedTimer));
                    upbeatStack.MapViewModel<RandomDataViewModel.Parameters, RandomDataViewModel, RandomDataControl>(
                        (upbeatService, parameters) => new RandomDataViewModel(upbeatService, new Random(), sharedTimer));
                    upbeatStack.MapViewModel<SharedListViewModel.Parameters, SharedListViewModel, SharedListControl>(
                        (upbeatService, parameters) =>
                        {
                            // The SharedListViewModel shares an IUpbeatService and scoped SharedList service with its child ViewModel, the SharedListDataViewModel. The scoped service can be manually created and provided to both.
                            var sharedList = new SharedList();
                            return new SharedListViewModel(upbeatService, sharedList, sharedTimer,
                                new SharedListDataViewModel(upbeatService, sharedList));
                        });
                    upbeatStack.MapViewModel<TextEntryPopupViewModel.Parameters, TextEntryPopupViewModel, TextEntryPopupControl>(
                        (upbeatService, parameters) => new TextEntryPopupViewModel(upbeatService, parameters, sharedTimer));

                    // The included UpdateMainWindow class already provides the necessary controls to display Views for ViewModels in a UpbeatStack set as the DataContext.
                    var mainWindow = new UpbeatMainWindow()
                    {
                        DataContext = upbeatStack,
                        Title = "UpbeatUI Sample Application",
                        MinHeight = 275,
                        MinWidth = 275,
                        Height = 400,
                        Width = 400,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        // The brush to display underneath the top View.
                        BlurColor = new SolidColorBrush(Brushes.OrangeRed.Color) { Opacity = 0.5 },
                    };

                    // Override the default Window Closing event to ensure that the UpbeatStack and all of its children ViewModels are properly disposed.
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
                    await upbeatStack.OpenViewModelAsync(new BottomViewModel.Parameters());

                    mainWindow.Closing -= closingHandler;
                    mainWindow.Close();
                }
            };
            app.Run();
        }
    }
}
