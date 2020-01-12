using System;

namespace UpbeatUI
{
    public class NegativeSelectableContext : SelectableContext
    {
        public NegativeSelectableContext(PositionContext positionContext, SizeContext sizeContext, Action select)
            : base(positionContext, sizeContext, select)
        { }
    }
}
