/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System.Windows;

namespace UpbeatUI.View
{
    /// <summary>
    /// Defines a <see cref="DependencyProperty"/> and property for a <see cref="Window"/> with an <see cref="OverlayDataContext"/> for an overlay View.
    /// </summary>
    public interface IOverlayWindow
    {
        /// <summary>
        /// Identifies the <see cref="OverlayDataContext"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty OverlayDataContextProperty =
            DependencyProperty.Register(
                "OverlayDataContext",
                typeof(object),
                typeof(UIElement),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a the data context for the <see cref="Window"/>'s optional overlay element.
        /// </summary>
        object OverlayDataContext { get; set; }
    }
}
