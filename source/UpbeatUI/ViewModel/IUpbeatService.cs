/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides methods for an UpbeatViewModel to interact with the UPbeatStack that it is a part of. IUpbeatServices are created by the UpbeatStack unique to each child IUpbeatViewModel, so do not share them.
    /// </summary>
    public interface IUpbeatService : IOpensUpbeatViewModels
    {
        /// <summary>
        /// Gets whether or not the View Model is the top item in the UpbeatStack, and thus active for the user.
        /// </summary>
        bool IsActiveUpbeatViewModel { get; }

        /// <summary>
        /// Signals to the parent UpbeatStack that this IUpbeatViewModel would like to close and be removed.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the current string contents of the clipboard. This is a convenience method for assemblies that do not want to reference System.Windows.
        /// </summary>
        /// <returns>The current contents of the clipboard.</returns>
        string GetClipboard();

        /// <summary>
        /// Sets the string contents of the clipboard. This is a convenience method for assemblies that do not want to reference System.Windows.
        /// </summary>
        /// <param name="text">What to set the clipboard to.</param>
        void SetClipboard(string text);
    }
}
