/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using UpbeatUI.Context;

namespace UpbeatUISample.ViewModel
{
    internal class PopupViewModel : ContextObject
    {
        protected readonly IContextService _contextService;

        public PopupViewModel(IContextService contextService, string message)
        {
            _contextService = contextService;
            Message = message;
        }

        public string Message { get; }
    }
}
