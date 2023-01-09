/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents a stack of ViewModels and provides methods and commands for controlling them. Uses an <see cref="IServiceProvider"/> to automatically resolve dependencies when creating ViewModels.
    /// </summary>
    public class ServiceProvidedUpbeatStack : UpbeatStack, IServiceProvidedUpbeatStack
    {
        private readonly IDictionary<Type, Func<IUpbeatService, object, object>> _childViewModelInstantiators = new Dictionary<Type, Func<IUpbeatService, object, object>>();
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<object, ViewModelScope> _viewModelParametersServiceScopes = new Dictionary<object, ViewModelScope>();
        private Action<Type> _autoMapper;

        /// <summary>
        /// Initializes an empty <see cref="ServiceProvidedUpbeatStack"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that the <see cref="ServiceProvidedUpbeatStack"/> will use to resolve dependencies when instantiating ViewModels.</param>
        /// <param name="updateOnRender">True to have the <see cref="UpbeatStack"/> execute any UppdateCallback <see cref="Action"/>s registered by ViewModels on each WPF frame render; false to only execute UpdateCallback <see cref="Action"/>s manually.</param>
        public ServiceProvidedUpbeatStack(IServiceProvider serviceProvider, bool updateOnRender = true)
            : base(updateOnRender) =>
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        public void MapViewModel<TParameters, TViewModel, TView>(bool allowUnresolvedDependencies)
            where TView : UIElement =>
            MapServiceProvidedViewModel<TParameters, TViewModel, TView>(allowUnresolvedDependencies);

        public void MapViewModel<TParameters, TViewModel, TView>(Func<IUpbeatService, TParameters, IServiceProvider, TViewModel> viewModelCreator)
            where TView : UIElement =>
            MapViewModel<TParameters, TViewModel, TView>((upbeatService, parameters) =>
                viewModelCreator(
                    upbeatService,
                    parameters,
                    _viewModelParametersServiceScopes[parameters].ServiceProvider));

        public override void OpenViewModel<TParameters>(TParameters parameters, Action closedCallback)
        {
            var parametersType = parameters.GetType();
            if (!_viewModelInstantiators.ContainsKey(parametersType))
                _autoMapper?.Invoke(parametersType);
            _viewModelParametersServiceScopes[parameters] = new ViewModelScope(_serviceProvider.CreateScope());
            base.OpenViewModel(parameters,
                () =>
                {
                    var viewModelScope = _viewModelParametersServiceScopes[parameters];
                    viewModelScope.Dispose();
                    _viewModelParametersServiceScopes.Remove(viewModelScope);
                    closedCallback?.Invoke();
                });
        }

        public void SetDefaultViewModelLocators(bool allowUnresolvedDependencies = false) =>
            SetViewModelLocators(
                (String parametersTypeString) => parametersTypeString.Replace("+Parameters", ""),
                (String parametersTypeString) => parametersTypeString.Replace("ViewModel+Parameters", "Control").Replace(".ViewModel.", ".View."),
                allowUnresolvedDependencies);

        public void SetViewModelLocators(Func<string, string> parameterToViewModelLocator,
                                           Func<string, string> parameterToViewLocator,
                                           bool allowUnresolvedDependencies = false) =>
            SetViewModelLocators(
                (Type parametersType) => Type.GetType(parameterToViewModelLocator(parametersType.AssemblyQualifiedName)),
                (Type parametersType) => Type.GetType(parameterToViewLocator(parametersType.AssemblyQualifiedName)),
                allowUnresolvedDependencies);

        public void SetViewModelLocators(Func<Type, Type> parameterToViewModelLocator,
                                           Func<Type, Type> parameterToViewLocator,
                                           bool allowUnresolvedDependencies = false)
        {
            _ = parameterToViewModelLocator ?? throw new ArgumentNullException(nameof(parameterToViewModelLocator));
            _ = parameterToViewLocator ?? throw new ArgumentNullException(nameof(parameterToViewLocator));
            _autoMapper = parametersType =>
            {
                var viewModelType = parameterToViewModelLocator(parametersType) ?? throw new InvalidOperationException($"Unable to locate ViewModel Type from Parameters Type: {parametersType.GetType().FullName}");
                var viewType = parameterToViewLocator(parametersType) ?? throw new InvalidOperationException($"Unable to locate View Type from Parameters Type: {parametersType.GetType().FullName}");
                var m = GetType().GetMethod(nameof(MapServiceProvidedViewModel), BindingFlags.NonPublic | BindingFlags.Instance);
                var n = m.MakeGenericMethod(parametersType, viewModelType, viewType);
                n.Invoke(this, new object[] { allowUnresolvedDependencies });
            };
        }

        private Func<IUpbeatService, object, object> CreateInstantiator(ConstructorInfo constructor, Type targetType,
                                                                        bool allowUnresolvedDependencies) =>
            new Func<IUpbeatService, object, object>((upbeatService, parameters) => Convert.ChangeType(
                constructor.Invoke(constructor.GetParameters()
                    .Select(p =>
                    {
                        if (p.ParameterType == typeof(IUpbeatService))
                            return upbeatService;
                        if (p.ParameterType == parameters.GetType())
                            return parameters;
                        var viewModelScope = _viewModelParametersServiceScopes[parameters];
                        var locatedService = viewModelScope.ServiceProvider.GetService(p.ParameterType);
                        if (locatedService != null)
                            return locatedService;
                        if (viewModelScope.ChildViewModels.TryGetValue(p.ParameterType, out var childViewModel))
                            return childViewModel;
                        childViewModel = ResolveDependency(p.ParameterType, allowUnresolvedDependencies, upbeatService, parameters);
                        if (childViewModel == null && !allowUnresolvedDependencies)
                            throw new InvalidOperationException($"No service for type '{p.ParameterType.FullName}' has been registered.");
                        viewModelScope.ChildViewModels[p.ParameterType] = childViewModel;
                        return childViewModel;
                    })
                    .ToArray()),
                targetType));

        private void MapServiceProvidedViewModel<TParameters, TViewModel, TView>(bool allowUnresolvedDependencies)
            where TView : UIElement
        {
            var viewModelType = typeof(TViewModel);
            var constructors = viewModelType.GetConstructors().ToList();
            if (constructors.Count > 1)
                throw new InvalidOperationException($"Type {viewModelType.FullName} has more than one constructor.");
            var constructor = constructors[0];
            var instantiator = CreateInstantiator(constructor, viewModelType, allowUnresolvedDependencies);
            MapViewModel<TParameters, TViewModel, TView>(
                (upbeatService, parameters) => (TViewModel)instantiator(upbeatService, parameters));
        }

        private object ResolveDependency(Type dependencyType, bool allowUnresolvedDependencies, IUpbeatService upbeatService, object parameters)
        {
            if (!_childViewModelInstantiators.TryGetValue(dependencyType, out var instantiator))
            {
                var constructors = dependencyType.GetConstructors().ToList();
                if (constructors.Count > 1)
                    return null;
                var constructor = constructors[0];
                var parameterTypes = constructor.GetParameters().Select(p => p.ParameterType);
                if (parameterTypes.Intersect(new[] { typeof(IUpbeatService), parameters.GetType() }).Count() == 0)
                    return null;
                instantiator = CreateInstantiator(constructor, dependencyType, allowUnresolvedDependencies);
                _childViewModelInstantiators[dependencyType] = instantiator;
            }
            return instantiator(upbeatService, parameters);
        }

        private class ViewModelScope : IDisposable
        {
            private IServiceScope _serviceScope;

            public ViewModelScope(IServiceScope serviceScope) =>
                _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));

            public IDictionary<Type, object> ChildViewModels { get; } = new Dictionary<Type, object>();
            public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

            public void Dispose()
            {
                foreach (var disposable in ChildViewModels.Values.OfType<IDisposable>().Reverse())
                    disposable.Dispose();
                _serviceScope.Dispose();
            }
        }
    }
}
