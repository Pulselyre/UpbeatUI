/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Threading.Tasks;

namespace UpbeatUI.ViewModel
{
    public interface IOpensUpbeatViewModels
    {
        /// <summary>
        /// Adds a new IUpbeatViewModel to the UpbeatStack based on the parameters provided.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create the IUpbeatViewModel.</typeparam>
        /// <param name="parameters">The parameters used to create the IUpbeatViewModel.</param>
        void OpenUpbeatViewModel<TParameters>(TParameters parameters);

        /// <summary>
        /// Adds a new IUpbeatViewModel to the UpbeatStack based on the parameters provided and executes a callback after that IUpbeatViewModel closes.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create the IUpbeatViewModel.</typeparam>
        /// <param name="parameters">The parameters used to create the IUpbeatViewModel.</param>
        /// <param name="closeCallback">A delegate for the UpbeatStack to execute after the IUpbeatViewModel closes.</param>
        void OpenUpbeatViewModel<TParameters>(TParameters parameters, Action closedCallback);

        /// <summary>
        /// Adds a new IUpbeatViewModel to the UpbeatStack based on the parameters provided and returns a Task that completes after the IUpbeatViewModel closes.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create the IUpbeatViewModel.</typeparam>
        /// <param name="parameters">The parameters used to create the IUpbeatViewModel.</param>
        /// <returns>A Task that represents the created IUpbeatViewModel being open</returns>
        Task OpenUpbeatViewModelAsync<TParameters>(TParameters parameters);
    }
}