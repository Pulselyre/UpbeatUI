/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using UpbeatUI.Extensions.DependencyInjection;
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
                // Use a ServiceCollection to set up dependencies that the ServiceProvidedUpbeatStack will inject into ViewModels. Scoped services are supported, and each ViewModel is a separate scope.
                var serviceCollection = new ServiceCollection();
                // Add dependent services here: serviceCollection.AddScoped(...

                // The ServiceProvidedUpbeatStack is the central data structure for an UpbeatUI app. One must be created for the life of the application and should be disposed at the end. Unlike the basic UpbeatStack, the ServiceProvidedUpbeatStack requires an IServiceProvider to resolve dependencies for ViewModels.
                using (var upbeatStack = new ServiceProvidedUpbeatStack(serviceCollection.BuildServiceProvider()))
                {
                    // Instead of manually mapping parameter types to ViewModels and controls, the ServiceProvidedUpbeatStack can automatically map types based on naming convention. Use this method to enable the default naming convention, but other methods enable you to use your own naming conventions.
                    upbeatStack.SetDefaultViewModelLocators();

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
