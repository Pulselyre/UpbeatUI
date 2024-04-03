/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Threading.Tasks;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Provides mechanisms for adding ViewModels to an <see cref="IUpbeatStack"/>.
    /// </summary>
    public interface IOpensViewModels

    {
        /// <summary>
        /// Adds a new ViewModel to the <see cref="IUpbeatStack"/> based on the <typeparamref name="TParameters"/> provided.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create the ViewModel.</typeparam>
        /// <param name="parameters">The parameters used to create the ViewModel.</param>
        void OpenViewModel<TParameters>(TParameters parameters);

        /// <summary>
        /// Adds a new ViewModel to the <see cref="IUpbeatStack"/> based on the <typeparamref name="TParameters"/> provided and executes a callback after that ViewModel closes.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create the ViewModel.</typeparam>
        /// <param name="parameters">The parameters used to create the ViewModel.</param>
        /// <param name="closedCallback">A delegate for the <see cref="IUpbeatStack"/> to execute after the ViewModel closes.</param>
        void OpenViewModel<TParameters>(TParameters parameters, Action closedCallback);

        /// <summary>
        /// Adds a new ViewModel to the <see cref="IUpbeatStack"/> based on the <typeparamref name="TParameters"/> provided and returns a <see cref="Task"/> that completes after the ViewModel closes.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create the ViewModel.</typeparam>
        /// <param name="parameters">The parameters used to create the ViewModel.</param>
        /// <returns>A <see cref="Task"/> that represents the created ViewModel being open</returns>
        Task OpenViewModelAsync<TParameters>(TParameters parameters);
    }
}
