/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;

namespace UpbeatUI.Context
{
    public class PositionContext : ObservableObject, IUpdatableContext
    {
        private Point _point;

        public PositionContext()
            : this(new Point(0.5, 0.5))
        { }

        public PositionContext(double xPosition, double yPosition)
            : this(new Point(xPosition, yPosition))
        { }

        public PositionContext(Point point) =>
            Point = point;

        public Point Point
        {
            get => _point;
            set { _point = value; RaisePropertyChanged(nameof(XPosition), nameof(YPosition), nameof(Point)); }
        }

        public double XPosition
        {
            get => Point.X;
            set { if (Point.X == value) return; Point = new Point(value, Point.Y); RaisePropertyChanged(nameof(XPosition), nameof(Point)); }
        }

        public double YPosition
        {
            get => Point.Y;
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
