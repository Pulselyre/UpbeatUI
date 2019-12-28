using System;
using System.Windows.Input;

namespace UpbeatUI
{
    public class SelectableContext : ObservableObject
    {
        public SelectableContext(PositionContext positionContext, Action select)
            :this(positionContext, new SizeContext(), select)
        { }

        public SelectableContext(PositionContext positionContext, SizeContext sizeContext, Action select)
        {
            PositionContext = positionContext;
            SizeContext = sizeContext;
            SelectCommand = new ObservableCommand(select);
        }

        public PositionContext PositionContext { get; }
        public ICommand SelectCommand { get; }
        public SizeContext SizeContext { get; }
    }
}
