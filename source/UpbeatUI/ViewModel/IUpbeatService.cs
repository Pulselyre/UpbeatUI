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
    public interface IUpbeatService : IOpensViewModels
    {
        /// <summary>
        /// Gets whether or not the ViewModel is the top item in the <see cref="UpbeatStack"/>, and thus active for the user.
        /// </summary>
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
        /// Registers a delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel (instead of closing it automatically). The delegate should return true if okay to close and false if the ViewModel needs to stay open. Multiple callbacks can be registered and the <see cref="UpbeatStack"/> will call each until the first false return value, or all return true.
        /// </summary>
        /// <param name="okToCloseCallback">The delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel. The delegate should return true if okay to close and false if the ViewModel needs to stay open.</param>
        void RegisterCloseCallback(Func<bool> okToCloseCallback);

        /// <summary>
        /// Registers an async delegate that the containing <see cref="UpbeatStack"/> will before closing this ViewModel (instead of closing it automatically). The async delegate should return true if okay to close and false if the ViewModel needs to stay open. Multiple callbacks can be registered and the <see cref="UpbeatStack"/> will call each until the first false return value, or all return true.
        /// </summary>
        /// <param name="asyncOkToCloseCallback">The async delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel. The async delegate should return true if okay to close and false if the ViewModel needs to stay open.</param>
        void RegisterCloseCallback(Func<Task<bool>> asyncOkToCloseCallback);

        /// <summary>
        /// Registers a delegate that the containing <see cref="UpbeatStack"/> will call on each frame render IF it is configured to do so. Call <see cref="UpdatesOnRender"/> to find out if this callback will be executed..
        /// </summary>
        /// <param name="updateCallback">THe delegate that the containing <see cref="UpbeatStack"/> will call on each frame render IF it is configured to do so. This delegate should execute as quickly as possible, otherwise it will affect UI responsiveness for the user.</param>
        void RegisterUpdateCallback(Action updateCallback);

        /// <summary>
        /// Sets the delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel (instead of closing it automatically). The delegate should return true if okay to close and false if the ViewModel needs to stay open.
        /// </summary>
        /// <param name="okToCloseCallback">The delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel. The delegate should return true if okay to close and false if the ViewModel needs to stay open.</param>
        [Obsolete("Method has been renamed to 'RegisterCloseCallback' which better describes how the UpbeatStack handles multiple okToCloseCallback. The 'SetCloseCallback' method will be removed in UpbeatUI 5.0.0.")]
        void SetCloseCallback(Func<bool> okToCloseCallback);

        /// <summary>
        /// Sets the async delegate that the containing <see cref="UpbeatStack"/> will before closing this ViewModel (instead of closing it automatically). The async delegate should return true if okay to close and false if the ViewModel needs to stay open.
        /// </summary>
        /// <param name="asyncOkToCloseCallback">The async delegate that the containing <see cref="UpbeatStack"/> will call before closing this ViewModel. The async delegate should return true if okay to close and false if the ViewModel needs to stay open.</param>
        [Obsolete("Method has been renamed to 'RegisterCloseCallback' which better describes how the UpbeatStack handles multiple asyncOkToCloseCallback. The 'SetCloseCallback' method will be removed in UpbeatUI 5.0.0.")]
        void SetCloseCallback(Func<Task<bool>> asyncOkToCloseCallback);

        /// <summary>
        /// Sets the delegate that the containing <see cref="UpbeatStack"/> will call on each frame render IF it is configured to do so. Call <see cref="UpdatesOnRender"/> to find out if this callback will be executed..
        /// </summary>
        /// <param name="updateCallback">THe delegate that the containing <see cref="UpbeatStack"/> will call on each frame render IF it is configured to do so. This delegate should execute as quickly as possible, otherwise it will affect UI responsiveness for the user.</param>
        [Obsolete("Method has been renamed to 'RegisterUpdateCallback' which better describes how the UpbeatStack handles multiple updateCallbacks. The 'SetUpdateCallback' method will be removed in UpbeatUI 5.0.0.")]
        void SetUpdateCallback(Action updateCallback);
    }
}
