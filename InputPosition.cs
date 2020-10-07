using System;
using System.Windows;

namespace UpbeatUI
{
    public class InputPosition
    {
        private Point _point;

        public InputPosition(int inputId, double xPosition, double yPosition)
        {
            if ((xPosition < 0.0) || (xPosition > 1.0))
                throw new ArgumentException("Argument for yPosition is not between 0 and 1.");
            if ((yPosition < 0.0) || (yPosition > 1.0))
                throw new ArgumentException("Argument for yPosition is not between 0 and 1.");
            _point = new Point(xPosition, yPosition);
            InputId = inputId;
        }

        public InputPosition(object[] array)
            : this((int)array[0], (double)array[1], (double)array[2])
        { }

        public InputPosition(object array)
            : this((object[])array) { }

        public int InputId { get; }
        public double XPosition { get { return _point.X; } }
        public double YPosition { get { return _point.Y; } }
    }
}
