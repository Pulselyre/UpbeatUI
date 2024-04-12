/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */

using System;
using System.Threading.Tasks;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides methods for a ViewModel to interact with the <see cref="IUpbeatStack"/> that it is a part of. <see cref="IUpbeatService"/> instances are created by the <see cref="IUpbeatStack"/> unique to each ViewModel, so do not share them.
    /// </summary>
    public interface IUpbeatService : IOpensViewModels
    {
        /// <summary>
        /// Gets whether or not the ViewModel is the top item in the <see cref="IUpbeatStack"/>, and thus active for the user.
        /// </summary>
        bool IsActiveViewModel { get; }
        /// <summary>
        /// Gets whether or not the parent <see cref="IUpbeatStack"/> is configured to update ViewModels on each rendering event.
        /// </summary>
        bool UpdatesOnRender { get; }

        /// <summary>
        /// Signals to the parent <see cref="IUpbeatStack"/> that this ViewModel would like to close and be removed. If <see cref="RegisterCloseCallback(Func{bool})"/> has been used, the callback will NOT be executed.
        /// </summary>
        void Close();

        /// <summary>
        /// Registers a delegate that the containing <see cref="IUpbeatStack"/> will call before closing this ViewModel (instead of closing it automatically). The delegate should return true if okay to close and false if the ViewModel needs to stay open. Multiple callbacks can be registered and the <see cref="IUpbeatStack"/> will call each until the first false return value, or all return true.
        /// </summary>
        /// <param name="okToCloseCallback">The delegate that the containing <see cref="IUpbeatStack"/> will call before closing this ViewModel. The delegate should return true if okay to close and false if the ViewModel needs to stay open.</param>
        void RegisterCloseCallback(Func<bool> okToCloseCallback);

        /// <summary>
        /// Registers an async delegate that the containing <see cref="IUpbeatStack"/> will call before closing this ViewModel (instead of closing it automatically). The async delegate should return true if okay to close and false if the ViewModel needs to stay open. Multiple callbacks can be registered and the <see cref="IUpbeatStack"/> will call each until the first false return value, or all return true.
        /// </summary>
        /// <param name="asyncOkToCloseCallback">The async delegate that the containing <see cref="IUpbeatStack"/> will call before closing this ViewModel. The async delegate should return true if okay to close and false if the ViewModel needs to stay open.</param>
        void RegisterCloseCallback(Func<Task<bool>> asyncOkToCloseCallback);

        /// <summary>
        /// Registers a delegate that the containing <see cref="IUpbeatStack"/> will call on each frame render IF it is configured to do so. Call <see cref="UpdatesOnRender"/> to find out if this callback will be executed..
        /// </summary>
        /// <param name="updateCallback">THe delegate that the containing <see cref="IUpbeatStack"/> will call on each frame render IF it is configured to do so. This delegate should execute as quickly as possible, otherwise it will affect UI responsiveness for the user.</param>
        void RegisterUpdateCallback(Action updateCallback);
    }
}
