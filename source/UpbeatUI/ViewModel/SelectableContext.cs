/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Windows.Input;

namespace UpbeatUI.ViewModel
{
    public class SelectableContext : ObservableObject, IUpdatableContext
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

        public void UpdateContextProperties()
        {
            PositionContext.UpdateContextProperties();
            SizeContext.UpdateContextProperties();
        }
    }
}
