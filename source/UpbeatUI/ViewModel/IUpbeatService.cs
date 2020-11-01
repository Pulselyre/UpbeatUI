/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */

using System;
using System.Threading.Tasks;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides methods for a ViewModel to interact with the <see cref="UpbeatStack"/> that it is a part of. <see cref="IUpbeatService"/> instancess are created by the <see cref="UpbeatStack"/> unique to each ViewModel, so do not share them.
    /// </summary>
    public interface IUpbeatService : IOpensViewModels, IOpensUpbeatViewModels
    {
        /// <summary>
        /// Gets whether or not the ViewModel is the top item in the <see cref="UpbeatStack"/>, and thus active for the user.
        /// </summary>
        [Obsolete("Renamed to IsActiveViewModel. This property will be removed in UpbeatUI 3.0.")]
        bool IsActiveUpbeatViewModel { get; }
        bool IsActiveViewModel { get; }
        /// <summary>
        /// Gets whether or not the parent <see cref="UpbeatStack"/> is configured to update ViewModels on each rendering event.
        /// </summary>
        bool UpdatesOnRender { get; }

        /// <summary>
        /// Signals to the parent <see cref="UpbeatStack"/> that this ViewModel would like to close and be removed. If a CloseCallback has been set, it will NOT be called.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets the current string contents of the clipboard. This is a convenience method for assemblies that do not want to reference <see cref="System.Windows"/>.
        /// </summary>
        /// <returns>The current contents of the clipboard.</returns>
        [Obsolete("This will be removed in UpbeatUI 3.0. Use another injected service instead.")]
        string GetClipboard();

        /// <summary>
        /// Sets the string contents of the clipboard. This is a convenience method for assemblies that do not want to reference <see cref="System.Windows"/>.
        /// </summary>
        /// <param name="text">What to set the clipboard to.</param>
        [Obsolete("This will be removed in UpbeatUI 3.0. Use another injected service instead.")]
        void SetClipboard(string text);

        /// <summary>
        /// Sets the delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel (instead of closing it automatically). The delegate should return true if okay to close and false if the ViewModel needs to stay open.
        /// </summary>
        /// <param name="okToCloseCallback">The delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel. The delegate should return true if okay to close and false if the ViewModel needs to stay open.</param>
        void SetCloseCallback(Func<bool> okToCloseCallback);

        /// <summary>
        /// Sets the async delegate that the containing <see cref="UpbeatStack"/> will before closing this ViewModel (instead of closing it automatically). The async delegate should return true if okay to close and false if the ViewModel needs to stay open.
        /// </summary>
        /// <param name="asyncOkToCloseCallback">The async delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel. The async delegate should return true if okay to close and false if the ViewModel needs to stay open.</param>
        void SetCloseCallback(Func<Task<bool>> asyncOkToCloseCallback);

        /// <summary>
        /// Sets the delegate that the containing <see cref="UpbeatStack"/> will call on each frame render IF it is configured to do so. Call <see cref="UpdatesOnRender"/> to find out if this callback will be executed..
        /// </summary>
        /// <param name="updateCallback">THe delegate that the containing <see cref="UpbeatStack"/> will call on each frame render IF it is configured to do so. This delegate should execute as quickly as possible, otherwise it will affect UI responsiveness for the user.</param>
        void SetUpdateCallback(Action updateCallback);
    }
}
