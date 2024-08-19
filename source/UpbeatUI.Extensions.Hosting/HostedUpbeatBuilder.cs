/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.ObjectModel;
using System.Windows;
using UpbeatUI.Extensions.DependencyInjection;
using UpbeatUI.View;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.Hosting
{
    internal sealed class HostedUpbeatBuilder : IHostedUpbeatBuilder
    {
        internal Func<object> BaseViewModelParametersCreator { get; private set; }
        internal Collection<Action<ServiceProvidedUpbeatStack>> MappingRegisterers { get; } = new Collection<Action<ServiceProvidedUpbeatStack>>();
        internal Func<Window> WindowCreator { get; private set; } = () => new UpbeatMainWindow();
        internal Action<IServiceProvider, Exception> FatalErrorHandler { get; private set; }

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

        public IHostedUpbeatBuilder MapViewModel<TParameters, TUpbeatViewModel>(bool allowUnresolvedDependencies = false)
        {
            MappingRegisterers.Add(
                upbeatStack => upbeatStack.MapViewModel<TParameters, TUpbeatViewModel>(allowUnresolvedDependencies));
            return this;
        }

        public IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel>(
            Func<IUpbeatService, TParameters, TViewModel> viewModelCreator)
        {
            MappingRegisterers.Add(
                upbeatStack => upbeatStack.MapViewModel(viewModelCreator));
            return this;
        }

        public IHostedUpbeatBuilder MapViewModel<TParameters, TViewModel>(
            Func<IUpbeatService, TParameters, IServiceProvider, TViewModel> viewModelCreator)
        {
            MappingRegisterers.Add(
                upbeatStack => upbeatStack.MapViewModel(viewModelCreator));
            return this;
        }

        public IHostedUpbeatBuilder SetDefaultViewModelLocators(bool allowUnresolvedDependencies = false)
        {
            MappingRegisterers.Add(
                upbeatStack => upbeatStack.SetDefaultViewModelLocators(allowUnresolvedDependencies));
            return this;
        }

        public IHostedUpbeatBuilder SetFatalErrorHandler(Action<IServiceProvider, Exception> fatalErrorHandler)
        {
            FatalErrorHandler = fatalErrorHandler;
            return this;
        }

        public IHostedUpbeatBuilder SetFatalErrorHandler(Action<Exception> fatalErrorHandler)
        {
            if (fatalErrorHandler == null)
            {
                fatalErrorHandler = null;
            }
            else
            {
                return SetFatalErrorHandler((_, e) => fatalErrorHandler(e));
            }
            return this;
        }

        public IHostedUpbeatBuilder SetViewModelLocators(
            Func<string, string> parameterToViewModelLocator,
            bool allowUnresolvedDependencies = false)
        {
            MappingRegisterers.Add(upbeatStack =>
                upbeatStack.SetViewModelLocators(
                    parameterToViewModelLocator,
                    allowUnresolvedDependencies));
            return this;
        }

        public IHostedUpbeatBuilder SetViewModelLocators(
            Func<Type, Type> parameterToViewModelLocator,
            bool allowUnresolvedDependencies = false)
        {
            MappingRegisterers.Add(upbeatStack =>
                upbeatStack.SetViewModelLocators(
                    parameterToViewModelLocator,
                    allowUnresolvedDependencies));
            return this;
        }
    }
}
