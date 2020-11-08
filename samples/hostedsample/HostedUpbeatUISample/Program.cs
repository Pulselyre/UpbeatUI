/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.Hosting;
using UpbeatUI.Extensions.Hosting;
using UpbeatUI.View;
using UpbeatUISample.ViewModel;

namespace UpbeatUISample
{
    public class Program
    {
        private static void Main(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureUpbeatHost(
                    () => new BottomViewModel.Parameters(), // Provide a delegate to create the bottom ViewModel. This is required.
                    builder => builder // The UpbeatStack depends on mappings of parameter types to ViewModels and controls to determine which ViewModel to create and which View to show.
                        .ConfigureWindow(() => new UpbeatMainWindow() // The included UpdateMainWindow class already provides the necessary controls to display Views for IViewModels. The UpbeatService will set the Window's DataContext automatically.
                        {
                            Title = "UpbeatUI Sample Application",
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                            BlurColor = new SolidColorBrush(Brushes.OrangeRed.Color) { Opacity = 0.5 }, // The brush to overlay Views underneath the top View.
                        }))
                .Build()
                .Run();
    }
}
