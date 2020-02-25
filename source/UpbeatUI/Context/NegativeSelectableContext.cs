/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.Context
{
    public class NegativeSelectableContext : SelectableContext
    {
        public NegativeSelectableContext(PositionContext positionContext, SizeContext sizeContext, Action select)
            : base(positionContext, sizeContext, select)
        { }
    }
}
