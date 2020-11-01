/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using UpbeatUI.ViewModel;

namespace UpbeatUISample.ViewModel
{
    internal class PopupViewModel
    {
        public PopupViewModel(IUpbeatService upbeatService, Parameters parameters) =>
            Message = parameters.Message;

        public string Message { get; }

        public class Parameters
        {
            public Parameters(string message) =>
                Message = message;

            public string Message { get; }
        }
    }
}
