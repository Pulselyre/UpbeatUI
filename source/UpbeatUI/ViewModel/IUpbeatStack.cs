/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
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
        /// Gets or sets an <see cref="Action"/> callback that the <see cref="UpbeatStack"/> will execute when it is empty of ViewModels.
        /// </summary>
        Action ViewModelsEmptyCallback { get; set; }

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="viewModelCreator">The delegate that will executed to create the ViewModel from an <see cref="IUpbeatService"/> and <typeparamref name="TParameters"/>.</param>
        void MapViewModel<TParameters, TViewModel, TView>(Func<IUpbeatService, TParameters, TViewModel> viewModelCreator) where TView : UIElement;

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type. The <see cref="IServiceProvider"/> will be used to resolve dependencies when creating the <typeparamref name="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="upbeatStack">The <see cref="UpbeatStack"/> to define the mapping on.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        void MapViewModel<TParameters, TViewModel, TView>(IServiceProvider serviceProvider) where TView : UIElement;

        /// <summary>
        /// Sets the <see cref="UpbeatStack"/> to automatically map Parameters <see cref="Type"/>s to ViewModel <see cref="Type"/>s and View <see cref="Type"/>s using the default conventions.
        /// <para>Parameters class names must follow the pattern of: "{BaseNamespace}.ViewModel.{Name}ViewModel+Parameters" (The Parameters class must be a public nested class of the ViewModel class).</para>
        /// <para>ViewModel class names must follow the pattern of: "{BaseNamespace}.ViewModel.{Name}ViewModel".</para>
        /// <para>View class names must follow the pattern of: "{BaseNamespace}.View.{Name}Control".</para>
        /// <para>For example: "Demo.ViewModel.MessageViewModel+Parameters", "Demo.ViewModel.MessageViewModel", and "Demo.View.MessageControl".</para>
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        void SetDefaultViewModelLocators(IServiceProvider serviceProvider);

        /// <summary>
        /// Sets delegates the <see cref="UpbeatStack"/> can use to automatically map a <see cref="string"/> representation of a Parameters <see cref="Type"/> to <see cref="string"/> represetantions of a ViewModel <see cref="Type"/> and a View <see cref="Type"/>.
        /// <para>Note: each <see cref="string"/> representation is a <see cref="Type.AssemblyQualifiedName"/></para>
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        /// <param name="parameterToViewModelLocator">A delegate to identify a <see cref="string"/> representation of a ViewModel <see cref="Type"/> from a <see cref="string"/> represetnation of a Parameters <see cref="Type"/>.
        /// <para>Note: each <see cref="string"/> representation is a <see cref="Type.AssemblyQualifiedName"/></para></param>
        /// <param name="parameterToViewLocator">A delegate to identify a <see cref="string"/> represetnation of a View <see cref="Type"/> from a <see cref="string"/> represetnation of a Parameters <see cref="Type"/>.
        /// <para>Note: The input <see cref="Type"/> is for the Parameters in the mapping, not the ViewModel.
        /// <para>Note: each <see cref="string"/> representation is a <see cref="Type.AssemblyQualifiedName"/></para></para></param>
        void SetViewModelLocators(IServiceProvider serviceProvider, Func<string, string> parameterToViewModelLocator, Func<string, string> parameterToViewLocator);

        /// <summary>
        /// Sets delegates the <see cref="UpbeatStack"/> can use to automatically map a Parameters <see cref="Type"/> to a ViewModel <see cref="Type"/> and a View <see cref="Type"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        /// <param name="parameterToViewModelLocator">A delegate to locate a ViewModel <see cref="Type"/> from a Parameters <see cref="Type"/>.</param>
        /// <param name="parameterToViewLocator">A delegate to locate a View <see cref="Type"/> from a Parameters <see cref="Type"/>.
        /// <para>Note: The input <see cref="Type"/> represents the Parameters in the mapping, not the ViewModel.</para></param>
        void SetViewModelLocators(IServiceProvider serviceProvider, Func<Type, Type> parameterToViewModelLocator, Func<Type, Type> parameterToViewLocator);

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