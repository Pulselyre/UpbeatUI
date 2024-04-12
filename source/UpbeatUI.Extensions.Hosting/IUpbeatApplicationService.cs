/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
namespace UpbeatUI.Extensions.Hosting
{
    /// <summary>
    /// Provides methods for a ViewModel to interact with the UpbeatUI <see cref="Microsoft.Extensions.Hosting.IHostedService"/> that it is executing the UpbeatUI application.
    /// </summary>
    public interface IUpbeatApplicationService
    {
        /// <summary>
        /// Tells the UpbeatUI <see cref="Microsoft.Extensions.Hosting.IHostedService"/> to try to stop the UpbeatUI application by attempting to close all open ViewModels. This is not guaranteed to be successful, as ViewModels can cancel close actions. This method returns immediately, regardless of success or how long it takes to close the application.
        /// </summary>
        void CloseUpbeatApplication();
    }
}
