/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using UpbeatUI.Extensions.DependencyInjection;
using UpbeatUI.View;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.Hosting
{
    internal sealed class HostedUpbeatBuilder : IHostedUpbeatBuilder
    {
        internal Func<object> BaseViewModelParametersCreator { get; private set; }
        internal Collection<Action<ServiceProvidedUpbeatStack>> MappingRegisterers { get; } = new Collection<Action<ServiceProvidedUpbeatStack>>();
        internal Func<IServiceProvider, IUpbeatStack, object, Window> WindowCreator { get; private set; }
        internal Action<IServiceProvider, Exception> FatalErrorHandler { get; private set; }
        internal Func<IServiceProvider, object> OverlayViewModelCreator { get; private set; }

        public IHostedUpbeatBuilder ConfigureWindow(Func<IServiceProvider, IUpbeatStack, object, Window> windowCreator)
        {
            WindowCreator = windowCreator;
            return this;
        }

        public IHostedUpbeatBuilder ConfigureWindow(Func<IServiceProvider, Window> windowCreator) =>
            ConfigureWindow(
                windowCreator == null ? null : new Func<IServiceProvider, IUpbeatStack, object, Window>(
                    (sp, us, ovm) =>
                    {
                        var window = windowCreator(sp);
                        window.DataContext = us;
                        if (window is IOverlayWindow overlayWindow)
                        {
                            overlayWindow.OverlayDataContext = ovm;
                        }
                        return window;
                    }));

        public IHostedUpbeatBuilder ConfigureWindow(Func<Window> windowCreator) =>
            ConfigureWindow(
                windowCreator == null ? null : new Func<IServiceProvider, Window>((_) => windowCreator()));

        public IHostedUpbeatBuilder ConfigureWindow<TWindow>()
            where TWindow : Window, new() =>
            ConfigureWindow(() => new TWindow());

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

        public IHostedUpbeatBuilder SetFatalErrorHandler(Action<Exception> fatalErrorHandler) =>
            SetFatalErrorHandler(fatalErrorHandler == null ? null : new Action<IServiceProvider, Exception>((_, e) => fatalErrorHandler(e)));

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

        public IHostedUpbeatBuilder SetOverlayViewModel(Func<IServiceProvider, object> overlayViewModelCreator)
        {
            OverlayViewModelCreator = overlayViewModelCreator;
            return this;
        }

        public IHostedUpbeatBuilder SetOverlayViewModel(Func<object> overlayViewModelCreator) =>
            SetOverlayViewModel(
                overlayViewModelCreator == null ? null : new Func<IServiceProvider, object>(_ => overlayViewModelCreator));

        public IHostedUpbeatBuilder SetOverlayViewModel<TOverlayViewModel>() =>
            SetOverlayViewModel(
                sp => sp.GetService(typeof(TOverlayViewModel)) ?? ActivatorUtilities.CreateInstance<TOverlayViewModel>(sp));
    }
}
