/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;

namespace UpbeatUI.Context
{
    public interface IContextService
    {
        bool IsActiveContext { get; }

        void Close();
        string GetClipboard();
        void OpenContext(ContextCreator contextCreator);
        void OpenContext(ContextCreator contextCreator, Action closedCallback);
        Task OpenContextAsync(ContextCreator contextCreator);
        void SetClipboard(string text);
    }
}
