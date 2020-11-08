/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Extensions.Hosting;
using UpbeatUI.View;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.Hosting
{
    internal class HostedUpbeatBuilder : IHostedUpbeatBuilder
    {
        private IHostApplicationLifetime _hostApplicationLifetime;
        private IServiceProvider _serviceProvider;

        public HostedUpbeatBuilder(IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
        }

        internal Func<object> BaseViewModelParametersCreator { get; private set; }
        internal IList<Action<UpbeatStack, IServiceProvider>> MappingRegisterers { get; } = new List<Action<UpbeatStack, IServiceProvider>>();
        internal Func<Window> WindowCreator { get; private set; } = () => new UpbeatMainWindow();

        public IHostedUpbeatService Build() =>
            new HostedUpbeatService(this, _serviceProvider, _hostApplicationLifetime);

        public IHostedUpbeatBuilder ConfigureWindow(Func<Window> windowCreator)
        {
            WindowCreator = windowCreator;
            return this;
        }

        public IHostedUpbeatBuilder ConfigureBaseViewModelParameters(Func<object> baseViewModelParametersCreator)
        {
            BaseViewModelParametersCreator = baseViewModelParametersCreator;
            return this;
        }

        public IHostedUpbeatBuilder MapViewModel<TParameters, TUpbeatViewModel, TView>()
            where TView : UIElement
        {
            MappingRegisterers.Add(
                (upbeatStack, serviceProvider) => upbeatStack.MapViewModel<TParameters, TUpbeatViewModel, TView>(serviceProvider));
            return this;
        }

        public IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel, TView>(Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
            where TView : UIElement
        {
            MappingRegisterers.Add(
                (upbeatStack, serviceProvider) => upbeatStack.MapViewModel<TParameters, TViewModel, TView>(viewModelCreator));
            return this;
        }

        public IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel, TView>(Func<IUpbeatService, TParameters, IServiceProvider, TViewModel> viewModelCreator)
            where TView : UIElement
        {
            MappingRegisterers.Add(
                (upbeatStack, serviceProvider) => upbeatStack.MapViewModel<TParameters, TViewModel, TView>(
                    (upbeatService, parameters) => viewModelCreator(upbeatService, parameters, serviceProvider)));
            return this;
        }

        public IHostedUpbeatBuilder SetDefaultViewModelLocators()
        {
            MappingRegisterers.Add(
                (upbeatStack, serviceProvider) => upbeatStack.SetDefaultViewModelLocators(serviceProvider));
            return this;
        }

        public IHostedUpbeatBuilder SetViewModelLocators(Func<string, string> parameterToViewModelLocator,
                                                         Func<string, string> parameterToViewLocator)
        {
            MappingRegisterers.Add((upbeatStack, serviceProvider) =>
                upbeatStack.SetViewModelLocators(serviceProvider,
                    parameterToViewModelLocator, parameterToViewLocator));
            return this;
        }

        public IHostedUpbeatBuilder SetViewModelLocators(Func<Type, Type> parameterToViewModelLocator,
                                                         Func<Type, Type> parameterToViewLocator)
        {
            MappingRegisterers.Add((upbeatStack, serviceProvider) =>
                upbeatStack.SetViewModelLocators(serviceProvider,
                    parameterToViewModelLocator, parameterToViewLocator));
            return this;
        }
    }
}
