/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Windows;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.Hosting
{
    /// <summary>
    /// A builder for the IHostedUpbeatService.
    /// </summary>
    public interface IHostedUpbeatBuilder
    {
        /// <summary>
        /// Sets a delegate for building the main <see cref="Window"/> that will house the UpbeatUI aplication. The <see cref="Window"/>'s DataContext will be set by the service.
        /// </summary>
        /// <param name="windowCreator">The delegate for creating the main <see cref="Window"/>.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder ConfigureWindow(Func<Window> windowCreator);

        /// <summary>
        /// Sets a delegate for building the bottom/base ViewModel.
        /// </summary>
        /// <param name="baseViewModelParametersCreator">The delegate for creating the bottom/base ViewModel</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder ConfigureBaseViewModelParameters(Func<object> baseViewModelParametersCreator);

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> <see cref="Type"/> and the <typeparamref name="TViewModel"/> <see cref="Type"/>. The <see cref="Microsoft.Extensions.Hosting.IHost"/> will be used to resolve dependencies when creating the <typeparamref name="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <param name="allowUnresolvedDependencies">If false, the <see cref="UpbeatStack"/> will throw an exception instead of providing a null service to the ViewModel.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel>(bool allowUnresolvedDependencies = false);

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> <see cref="Type"/> and the <typeparamref name="TViewModel"/> <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <param name="viewModelCreator">The delegate that will executed to create the ViewModel from an <see cref="IUpbeatService"/> and <typeparamref name="TParameters"/>.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel>(
            Func<IUpbeatService, TParameters, TViewModel> viewModelCreator);

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> <see cref="Type"/> and the <typeparamref name="TViewModel"/> <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <param name="viewModelCreator">The delegate that will executed to create the ViewModel from an <see cref="IUpbeatService"/>, <typeparamref name="TParameters"/>, and <see cref="IServiceProvider"/>.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel>(
            Func<IUpbeatService, TParameters, IServiceProvider, TViewModel> viewModelCreator);

        /// <summary>
        /// Sets the <see cref="UpbeatStack"/> to automatically map Parameters <see cref="Type"/>s to ViewModel <see cref="Type"/>s and View <see cref="Type"/>s using the default conventions.
        /// <para>Parameters class names must follow the pattern of: "{BaseNamespace}.ViewModel.{Name}ViewModel+Parameters" (The Parameters class must be a public nested class of the ViewModel class).</para>
        /// <para>ViewModel class names must follow the pattern of: "{BaseNamespace}.ViewModel.{Name}ViewModel".</para>
        /// <para>View class names must follow the pattern of: "{BaseNamespace}.View.{Name}Control".</para>
        /// <para>For example: "Demo.ViewModel.MessageViewModel+Parameters", "Demo.ViewModel.MessageViewModel", and "Demo.View.MessageControl".</para>
        /// </summary>
        /// <param name="allowUnresolvedDependencies">If false, the <see cref="UpbeatStack"/> will throw an exception instead of providing a null service to the ViewModel.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder SetDefaultViewModelLocators(bool allowUnresolvedDependencies = false);

        /// <summary>
        /// Sets delegates the <see cref="UpbeatStack"/> can use to automatically map a <see cref="string"/> representation of a Parameters <see cref="Type"/> to <see cref="string"/> represetantions of a ViewModel <see cref="Type"/> and a View <see cref="Type"/>.
        /// <para>Note: Each <see cref="string"/> representation is a <see cref="Type.AssemblyQualifiedName"/></para>
        /// </summary>
        /// <param name="parameterToViewModelLocator">A delegate to identify a <see cref="string"/> representation of a ViewModel <see cref="Type"/> from a <see cref="string"/> represetnation of a Parameters <see cref="Type"/>.
        /// <para>Note: Each <see cref="string"/> representation is a <see cref="Type.AssemblyQualifiedName"/></para>
        /// <para>Note: The input <see cref="string"/> is for the Parameters in the mapping, not the ViewModel.</para>
        /// </param>
        /// <param name="allowUnresolvedDependencies">If false, the <see cref="UpbeatStack"/> will throw an exception instead of providing a null service to the ViewModel.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns><see cref="Type"/>
        IHostedUpbeatBuilder SetViewModelLocators(
            Func<string, string> parameterToViewModelLocator,
            bool allowUnresolvedDependencies = false);

        /// <summary>
        /// Sets delegates the <see cref="UpbeatStack"/> can use to automatically map a Parameters <see cref="Type"/> to a ViewModel <see cref="Type"/> and a View <see cref="Type"/>.
        /// </summary>
        /// <param name="parameterToViewModelLocator">A delegate to locate a ViewModel <see cref="Type"/> from a Parameters <see cref="Type"/>.
        /// <para>Note: The input <see cref="Type"/> represents the Parameters in the mapping, not the ViewModel.</para>
        /// </param>
        /// <param name="allowUnresolvedDependencies">If false, the <see cref="UpbeatStack"/> will throw an exception instead of providing a null service to the ViewModel.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder SetViewModelLocators(
            Func<Type, Type> parameterToViewModelLocator,
            bool allowUnresolvedDependencies = false);
    }
}
