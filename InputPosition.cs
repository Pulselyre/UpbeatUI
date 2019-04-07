using static System.Math;
using System.Windows;

namespace UpbeatUI
{
    public class InputPosition
    {
        public InputPosition(int inputId, double xPosition, double yPosition)
        {
            Point = new Point(
                Min(1.0, Max(0.0, xPosition)),
                Min(1.0, Max(0.0, yPosition)));
            InputId = inputId;
        }

        public InputPosition(object[] array)
            : this((int)array[0], (double)array[1], (double)array[2])
        { }

        public InputPosition(object array)
            : this((object[])array) { }

        public int InputId { get; }
        public Point Point { get; }
        public double XPosition { get { return Point.X; } }
        public double YPosition { get { return Point.Y; } }
    }
}
