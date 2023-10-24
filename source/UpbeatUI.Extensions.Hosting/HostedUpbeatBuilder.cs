/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Extensions.Hosting;
using UpbeatUI.Extensions.DependencyInjection;
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
        internal IList<Action<ServiceProvidedUpbeatStack>> MappingRegisterers { get; } = new List<Action<ServiceProvidedUpbeatStack>>();
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

        public IHostedUpbeatBuilder MapViewModel<TParameters, TUpbeatViewModel, TView>(bool allowUnresolvedDependencies = false)
            where TView : UIElement
        {
            MappingRegisterers.Add(
                upbeatStack => upbeatStack.MapViewModel<TParameters, TUpbeatViewModel, TView>(allowUnresolvedDependencies));
            return this;
        }

        public IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel, TView>(Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
            where TView : UIElement
        {
            MappingRegisterers.Add(
                upbeatStack => upbeatStack.MapViewModel<TParameters, TViewModel, TView>(viewModelCreator));
            return this;
        }

        public IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel, TView>(Func<IUpbeatService, TParameters, IServiceProvider, TViewModel> viewModelCreator)
            where TView : UIElement
        {
            MappingRegisterers.Add(
                upbeatStack => upbeatStack.MapViewModel<TParameters, TViewModel, TView>(viewModelCreator));
            return this;
        }

        public IHostedUpbeatBuilder SetDefaultViewModelLocators(bool allowUnresolvedDependencies = false)
        {
            MappingRegisterers.Add(
                upbeatStack => upbeatStack.SetDefaultViewModelLocators(allowUnresolvedDependencies));
            return this;
        }

        public IHostedUpbeatBuilder SetViewModelLocators(Func<string, string> parameterToViewModelLocator,
                                                         Func<string, string> parameterToViewLocator,
                                                         bool allowUnresolvedDependencies = false)
        {
            MappingRegisterers.Add(upbeatStack =>
                upbeatStack.SetViewModelLocators(
                    parameterToViewModelLocator,
                    parameterToViewLocator,
                    allowUnresolvedDependencies));
            return this;
        }

        public IHostedUpbeatBuilder SetViewModelLocators(Func<Type, Type> parameterToViewModelLocator,
                                                         Func<Type, Type> parameterToViewLocator,
                                                         bool allowUnresolvedDependencies = false)
        {
            MappingRegisterers.Add(upbeatStack =>
                upbeatStack.SetViewModelLocators(
                    parameterToViewModelLocator,
                    parameterToViewLocator,
                    allowUnresolvedDependencies));
            return this;
        }
    }
}
