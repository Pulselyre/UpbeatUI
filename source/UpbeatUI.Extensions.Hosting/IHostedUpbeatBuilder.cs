/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
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
        /// Builds an <see cref="IHostedUpbeatService"/> which hosts an UpbeatUI application.
        /// </summary>
        /// <returns>A configured <see cref="IHostedUpbeatService"/> ready to be started.</returns> 
        IHostedUpbeatService Build();

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
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type. The <see cref="Microsoft.Extensions.Hosting.IHost"/> will be used to resolve dependencies when creating the <typeparamref name="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel, TView>()
            where TView : UIElement;

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="viewModelCreator">The delegate that will executed to create the ViewModel from an <see cref="IUpbeatService"/> and <typeparamref name="TParameters"/>.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel, TView>(
            Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
            where TView : UIElement;

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="viewModelCreator">The delegate that will executed to create the ViewModel from an <see cref="IUpbeatService"/>, <typeparamref name="TParameters"/>, and <see cref="IServiceProvider"/>.</param>
        /// <returns>The <see cref="IHostedUpbeatBuilder"/> for chaining.</returns>
        IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel, TView>(
            Func<IUpbeatService, TParameters, IServiceProvider, TViewModel> viewModelCreator)
            where TView : UIElement;
    }
}
