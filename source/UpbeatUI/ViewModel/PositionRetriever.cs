/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows;
using UpbeatUI.View;

namespace UpbeatUI.ViewModel
{
    public class PositionRetriever
    {
        public Point Point => Retriever?.Invoke() ?? throw new InvalidCastException($"This {nameof(PositionRetriever)} has not been bound to an {nameof(AttachedSizeAndPosition.PositionRetrieverProperty)}.");

        internal Func<Point> Retriever { get; set; }
    }
}
