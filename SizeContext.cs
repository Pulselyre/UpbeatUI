using System.Windows;

namespace UpbeatUI
{
    public class SizeContext : ObservableObject
    {
        public SizeContext(double height, double width)
        {
            Size = new Size(height, width);
        }

        public SizeContext()
            : this(0, 0)
        { }

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
    }
}
