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
                // The ContextStack is the central data structure for an UpbeatUI app. One must be created for the life of the application and should be disposed at the end.
                using (var contextStack = new ContextStack())
                {
                    // The ContextStack depends on mappings of contexts to controls to determine which View to show for which View Model.
                    contextStack.SetContextControlMappings(
                        new Dictionary<Type, Type>()
                        {
                            [typeof(BottomViewModel)] = typeof(BottomControl),
                            [typeof(MenuViewModel)] = typeof(MenuControl),
                            [typeof(PopupViewModel)] = typeof(PopupControl),
                            [typeof(PositionedPopupViewModel)] = typeof(PositionedPopupControl),
                            [typeof(ScaledPopupViewModel)] = typeof(ScaledPopupControl),
                        });

                    // The ContextStack can execute Update methods on View Models that implement IUpdatableContext. Subscribing the ContextStack's RenderingHandler to the CompositionTarget.Rendering event ensures updates are executed once per frame draw.
                    // (Note, the update feature is not demonstrated in this sample application.)
                    CompositionTarget.Rendering += contextStack.RenderingHandler;

                    // The included UpdateMainWindow class already provides the necessary controls to display Views for View Models in a ContextStack set as the DataContext.
                    var mainWindow = new UpbeatMainWindow()
                    {
                        DataContext = contextStack,
                        Title = "UpbeatUI Sample Application",
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        BlurColor = new SolidColorBrush(Brushes.OrangeRed.Color) { Opacity = 0.5 }, // The brush to overlay Views underneath the top View.
                    };

                    // Override the default Window Closing event to ensure that the ContextStack and all of its children ViewModels are properly disposed.
                    CancelEventHandler closingHandler = (sender, e) => { e.Cancel = true; contextStack.RemoveAllContexts(); };
                    mainWindow.Closing += closingHandler;

                    mainWindow.Show();

                    // Add a base ViewModel to the ContextStack and wait for it to be closed.
                    await contextStack.OpenContextAsync(service => new BottomViewModel(service, contextStack.RemoveAllContexts));

                    mainWindow.Closing -= closingHandler;
                    CompositionTarget.Rendering -= contextStack.RenderingHandler;
                    mainWindow.Close();
                }
            };
            app.Run();
        }
    }
}
