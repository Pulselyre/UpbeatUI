/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace UpbeatUI.View
{
    /// <summary>
    /// Defines a window with a pre-embedded <see cref="ModalPanel"> to stack Views. The <see cref="DataContext"> should be set to an <see cref="UpbeatStack"> instance.
    /// </summary>
    public partial class UpbeatMainWindow : Window
    {
        /// <summary>
        /// Identifies the <see cref="ModalBlurEffect"/> <see cref="DependencyProperty">.
        /// </summary>
        public readonly static DependencyProperty ModalBackgroundProperty =
            ModalPanel.ModalBackgroundProperty.AddOwner(typeof(UpbeatMainWindow));

        public readonly static DependencyProperty ModalBlurEffectProprety =
            DependencyProperty.Register(
                "ModalBlurEffect",
                typeof(BlurEffect),
                typeof(UpbeatMainWindow),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Initializes a new <see cref="UpbeatMainWindow">.
        /// </summary>
        public UpbeatMainWindow() =>
            InitializeComponent();

        /// <summary>
        /// Gets or sets a <see cref="Brush"> that the <see cref="ModalPanel"> will show underneath the top (active) Element.
        /// </summary>
        public Brush ModalBackground
        {
            get => (Brush)GetValue(ModalBackgroundProperty);
            set => SetValue(ModalBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets a <see cref="BlurEffect"/> that the <see cref="ModalPanel"/> will apply to underneath (non-active) Elements.
        /// </summary>
        public BlurEffect ModalBlurEffect
        {
            get => (BlurEffect)GetValue(ModalBlurEffectProprety);
            set => SetValue(ModalBlurEffectProprety, value);
        }
    }
}
