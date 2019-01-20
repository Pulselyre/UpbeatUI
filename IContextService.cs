namespace UpbeatUI
{
    public interface IContextService
    {
        void Close();
        string GetClipboard();
        void OpenContext(ContextCreator contextCreator);
        void SetClipboard(string text);
    }
}
