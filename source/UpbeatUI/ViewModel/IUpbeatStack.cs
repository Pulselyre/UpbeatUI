/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Represents a stack of ViewModels and provides methods and commands for controlling them.
    /// </summary>
    public interface IUpbeatStack : IOpensViewModels
    {
        /// <summary>
        /// <see cref="EventHandler"/> that the <see cref="IUpbeatStack"/> will fire when it is empty of ViewModels.
        /// </summary>
        event EventHandler ViewModelsEmptied;

        /// <summary>
        /// Gets the count of the <see cref="IUpbeatStack"/>'s current ViewModels.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets a command to remove the top (active) ViewModel.
        /// </summary>
        ICommand RemoveTopViewModelCommand { get; }
        /// <summary>
        /// Gets the <see cref="IUpbeatStack"/>'s current ViewModels.
        /// </summary>
        INotifyCollectionChanged ViewModels { get; }

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> <see cref="Type"/> and the <typeparamref name="TViewModel"/> <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <param name="viewModelCreator">The delegate that be will executed to create the ViewModel from an <see cref="IUpbeatService"/> and <typeparamref name="TParameters"/>.</param>
        void MapViewModel<TParameters, TViewModel>(Func<IUpbeatService, TParameters, TViewModel> viewModelCreator);

        /// <summary>
        /// Tries to close and dispose all open ViewModels from the <see cref="IUpbeatStack"/>.
        /// </summary>
        /// <returns>A task that represents the process of closing all ViewModels with a result of whether it was successful or not.</returns>
        Task<bool> TryCloseAllViewModelsAsync();

        /// <summary>
        /// Executes any update callback methods that ViewModels have registered with <see cref="IUpbeatService.RegisterUpdateCallback"/>. Only executes if the <see cref="IUpbeatStack"/> is set to NOT update on render; otherwise is a noop.
        /// </summary>
        void UpdateViewModelProperties();
    }
}
