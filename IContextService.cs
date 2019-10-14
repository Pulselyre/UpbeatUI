using System;

namespace UpbeatUI
{
    public interface IContextService
    {
        void Close();
        string GetClipboard();
        void OpenContext(ContextCreator contextCreator);
        void OpenContext(ContextCreator contextCreator, Action closedCallback);
        void SetClipboard(string text);
    }
}
