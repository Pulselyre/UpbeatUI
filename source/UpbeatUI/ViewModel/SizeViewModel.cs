/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;

namespace UpbeatUI.ViewModel
{
    public class SizeViewModel : BaseViewModel, IUpdatableViewModel
    {
        public SizeViewModel()
            : this(0, 0)
        { }

        public SizeViewModel(double width, double height)
            : this(new Size(width, height))
        { }

        public SizeViewModel(Size size)
        {
            Size = size;
        }

        public Size Size { get; private set; }

        public double Height
        {
            get { return Size.Height; }
            set { if (Size.Height == value) return; Size = new Size(Size.Width, value); RaisePropertyChanged(nameof(Height)); }
        }

        public double Width
        {
            get { return Size.Width; }
            set { if (Size.Width == value) return; Size = new Size(value, Size.Height); RaisePropertyChanged(nameof(Width)); }
        }

        public Func<Size> Finder { get; internal set; }

        public void Change(Size size) =>
            Change(size.Width, size.Height);

        public void Change(double width, double height)
        {
            if (Size.Width == width && Size.Height == height)
                return;
            Size = new Size(width, height);
            RaisePropertyChanged(nameof(width), nameof(height), nameof(Point));
        }

        public void UpdateViewModelProperties() =>
            Change(Finder?.Invoke() ?? new Size());
    }
}
