/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;

namespace UpbeatUI.ViewModel
{
    public class NegativeSelectableViewModel : SelectableViewModel
    {
        public NegativeSelectableViewModel(PositionViewModel positionViewModel, SizeViewModel sizeViewModel, Action select)
            : base(positionViewModel, sizeViewModel, select)
        { }
    }
}
