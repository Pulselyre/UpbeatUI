/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.Hosting
{
    public static class Extensions
    {
        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type. The <see cref="IHost"/> will be used to resolve dependencies when creating the <typeparamref name="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="upbeatStack">The <see cref="UpbeatStack"/> to define the mapping on.</param>
        /// <param name="host">The <see cref="IHost"/> that will be used to resolve dependencies.</param>
        public static void MapViewModel<TParameters, TViewModel, TView>(this UpbeatStack upbeatStack, IHost host)
            where TView : UIElement =>
            upbeatStack.MapViewModel<TParameters, TViewModel, TView>(host.Services);

        /// <summary>
        /// Defines a mapping between the <typeparamref name="TParameters"/> type, the <typeparamref name="TViewModel"/> Type and the <typeparamref name="TView"/> Type. The <see cref="IServiceProvider"/> will be used to resolve dependencies when creating the <typeparamref name="TViewModel"/>.
        /// </summary>
        /// <typeparam name="TParameters">The type of the parameters used to create <typeparamref name="TViewModel"/>s.</typeparam>
        /// <typeparam name="TViewModel">The type of the ViewModel created from a <typeparamref name="TParameters"/>.</typeparam>
        /// <typeparam name="TView">The Type of the <see cref="UIElement"/>.</typeparam>
        /// <param name="upbeatStack">The <see cref="UpbeatStack"/> to define the mapping on.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that will be used to resolve dependencies.</param>
        public static void MapViewModel<TParameters, TViewModel, TView>(this UpbeatStack upbeatStack, IServiceProvider serviceProvider)
            where TView : UIElement
        {
            var constructors = typeof(TViewModel).GetConstructors().ToList();
            if (constructors.Count > 1)
                throw new InvalidOperationException($"Type {typeof(TViewModel).Name} has more than one constructor.");
            var constructor = constructors[0];
            var serviceType = typeof(IUpbeatService);
            upbeatStack.MapViewModel<TParameters, TViewModel, TView>(
                (service, parameters) => (TViewModel)constructor.Invoke(constructor.GetParameters().Select(
                        p => p.ParameterType == typeof(IUpbeatService) ? service
                            : p.ParameterType == typeof(TParameters) ? parameters
                            : serviceProvider.GetRequiredService(p.ParameterType)).ToArray()));
        }
    }
}
