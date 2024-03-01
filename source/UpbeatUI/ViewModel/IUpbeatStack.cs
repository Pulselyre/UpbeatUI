/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UpbeatUI.ViewModel
{
    /// <summary>
    /// Represents a stack of ViewModels and provides methods and commands for controlling them.
    /// </summary>
    public interface IUpbeatStack : IOpensViewModels
    {
        /// <summary>
        /// <see cref="EventHandler"/> that the <see cref="UpbeatStack"/> will fire when it is empty of ViewModels.
        /// </summary>
        event EventHandler ViewModelsEmptied;

        /// <summary>
        /// Gets the count of the <see cref="UpbeatStack"/>'s current ViewModels.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets a command to remove the top (active) ViewModel.
        /// </summary>
        ICommand RemoveTopViewModelCommand { get; }
        /// <summary>
        /// Gets the <see cref="UpbeatStack"/>'s current ViewModels.
        /// </summary>
        INotifyCollectionChanged ViewModels { get; }

        /// <summary>
        /// Gets a View <see cref="Type"/> that is mapped to the <paramref name="viewModelType"/> <see cref="Type"/>.
        /// </summary>
        /// <param name="viewModelType">The ViewModel <see cref="Type"/> to get the mapped View <see cref="Type"/> for.</param>
        /// <returns>The mapped View <see cref="Type"/> or null if not mapped.</returns>
        public Type GetViewTypeFromViewModelType(Type viewModelType);

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="viewModelCreator">The delegate that will executed to create the ViewModel from an <see cref="IUpbeatService"/> and <typeparamref name="TParameters"/>.</param>
        void MapViewModel<TParameters, TViewModel, TView>(Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
            where TView : UIElement;

        /// <summary>
        /// Tries to close and dispose all open ViewModels from the <see cref="UpbeatStack"/>.
        /// </summary>
        /// <returns>A task that represents the process of closing all ViewModels with a result of whether it was successful or not.</returns>
        Task<bool> TryCloseAllViewModelsAsync();

        /// <summary>
        /// Executes the <see cref="UpdateViewModelProperties"/> method on each ViewModel that implements the <see cref="IUpdatableViewModel"/> interface. Only executes if the <see cref="UpbeatStack"/> is set to NOT update on render; otherwise is a noop.
        /// </summary>
        void UpdateViewModelProperties();
    }
}
