using System;
using System.Threading.Tasks;

namespace UpbeatUI
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
