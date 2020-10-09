/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    internal class ScaledPopupViewModel : PopupViewModel
    {
        public ScaledPopupViewModel(IUpbeatService upbeatService, string message)
            : base(upbeatService, message)
        { }

        public new class Parameters : PopupViewModel.Parameters
        {
            public Parameters(string message)
                : base(message)
            { }
        }
    }
}
