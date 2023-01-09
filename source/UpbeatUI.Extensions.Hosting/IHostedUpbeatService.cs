/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using Microsoft.Extensions.Hosting;

namespace UpbeatUI.Extensions.Hosting
{
    /// <summary>
    /// Defines methods for an UpbeatUI application that is managed by the host.
    /// </summary>
    public interface IHostedUpbeatService : IUpbeatApplicationService, IHostedService
    { }
}
