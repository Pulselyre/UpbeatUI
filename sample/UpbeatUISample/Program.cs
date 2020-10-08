/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using UpbeatUI.ViewModel;
using UpbeatUI.View;
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
                    // The UpbeatStack depends on mappings of IUpbeatViewModels to controls to determine which View to show for which UpbeatViewModels.
                    upbeatStack.SetUpbeatViewModelControlMappings(
                        new Dictionary<Type, Type>()
                        {
                            [typeof(BottomViewModel)] = typeof(BottomControl),
                            [typeof(MenuViewModel)] = typeof(MenuControl),
                            [typeof(PopupViewModel)] = typeof(PopupControl),
                            [typeof(PositionedPopupViewModel)] = typeof(PositionedPopupControl),
                            [typeof(ScaledPopupViewModel)] = typeof(ScaledPopupControl),
                        });

                    // The UpbeatStack can execute the UpdateViewModelProperties method on IUpbeatViewModels that implement IUpdatableViewModel. Subscribing the UpbeatStack's RenderingHandler to the CompositionTarget.Rendering event ensures updates are executed once per frame draw.
                    // (Note, the update feature is not demonstrated in this sample application.)
                    CompositionTarget.Rendering += upbeatStack.RenderingHandler;

                    // The included UpdateMainWindow class already provides the necessary controls to display Views for IUpbeatViewModels in a UpbeatStack set as the DataContext.
                    var mainWindow = new UpbeatMainWindow()
                    {
                        DataContext = upbeatStack,
                        Title = "UpbeatUI Sample Application",
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        BlurColor = new SolidColorBrush(Brushes.OrangeRed.Color) { Opacity = 0.5 }, // The brush to overlay Views underneath the top View.
                    };

                    // Override the default Window Closing event to ensure that the UpbeatStack and all of its children IUpbeatViewModels are properly disposed.
                    CancelEventHandler closingHandler = (sender, e) => { e.Cancel = true; upbeatStack.RemoveAllUpbeatViewModels(); };
                    mainWindow.Closing += closingHandler;

                    mainWindow.Show();

                    // Add a base BottomViewModel to the UpbeatStack and wait for it to be closed.
                    await upbeatStack.OpenUpbeatViewModelAsync(service => new BottomViewModel(service, upbeatStack.RemoveAllUpbeatViewModels));

                    mainWindow.Closing -= closingHandler;
                    CompositionTarget.Rendering -= upbeatStack.RenderingHandler;
                    mainWindow.Close();
                }
            };
            app.Run();
        }
    }
}
