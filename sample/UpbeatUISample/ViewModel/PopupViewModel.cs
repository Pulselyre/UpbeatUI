/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    internal class PopupViewModel : UpbeatViewModel
    {
        protected readonly IUpbeatService _upbeatService;

        public PopupViewModel(IUpbeatService upbeatService, string message)
        {
            _upbeatService = upbeatService;
            Message = message;
        }

        public string Message { get; }
    }
}
