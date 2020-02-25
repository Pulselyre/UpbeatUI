/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using UpbeatUI.Context;
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
                using (var contextStack = new ContextStack())
                {
                    contextStack.SetContextControlMappings(
                        new Dictionary<Type, Type>()
                        {
                            [typeof(BottomViewModel)] = typeof(BottomControl),
                            [typeof(MenuViewModel)] = typeof(MenuControl),
                            [typeof(PopupViewModel)] = typeof(PopupControl),
                            [typeof(PositionedPopupViewModel)] = typeof(PositionedPopupControl),
                            [typeof(ScaledPopupViewModel)] = typeof(ScaledPopupControl),
                        });
                    CompositionTarget.Rendering += contextStack.RenderingHandler;
                    var mainWindow = new UpbeatMainWindow()
                    {
                        DataContext = contextStack,
                        Title = "UpbeatUI Sample Application",
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        BlurColor = new SolidColorBrush(Brushes.OrangeRed.Color) { Opacity = 0.5 },
                    };
                    CancelEventHandler closingHandler = (sender, e) => { e.Cancel = true; contextStack.RemoveAllContexts(); };
                    mainWindow.Closing += closingHandler;
                    mainWindow.Show();
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
