using System;
using System.Windows;

namespace UpbeatUI
{
    public class PositionContext : ObservableObject, IUpdatableContext
    {
        public PositionContext()
            : this(new Point(0.5, 0.5))
        { }

        public PositionContext(double xPosition, double yPosition)
            : this(new Point(xPosition, yPosition))
        { }

        public PositionContext(Point point)
        {
            Point = point;
        }

        public Point Point { get; private set; }

        public double XPosition
        {
            get { return Point.X; }
            set { if (Point.X == value) return; Point = new Point(value, Point.Y); RaisePropertyChanged(nameof(XPosition), nameof(Point)); }
        }

        public double YPosition
        {
            get { return Point.Y; }
            set { if (Point.Y == value) return; Point = new Point(Point.X, value); RaisePropertyChanged(nameof(YPosition), nameof(Point)); }
        }

        public Func<Point> Finder { get; internal set; }

        public void Change(Point point) =>
            Change(point.X, point.Y);

        public void Change(double xPosition, double yPosition)
        {
            if (Point.X == xPosition && Point.Y == yPosition)
                return;
            Point = new Point(xPosition, yPosition);
            RaisePropertyChanged(nameof(xPosition), nameof(YPosition), nameof(Point));
        }

        public void UpdateContextProperties() =>
            Change(Finder?.Invoke() ?? new Point());
    }
}
