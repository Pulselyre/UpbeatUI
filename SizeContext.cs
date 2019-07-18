using System.Windows;

namespace UpbeatUI
{
    public class SizeContext : ObservableObject
    {
        public SizeContext()
            : this(0, 0)
        { }

        public SizeContext(double width, double height)
            : this(new Size(width, height))
        { }

        public SizeContext(Size size)
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

        public void Change(double width, double height)
        {
            if (Size.Width == width && Size.Height == height)
                return;
            Size = new Size(width, height);
            RaisePropertyChanged(nameof(width), nameof(height), nameof(Point));
        }
    }
}
