/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UpbeatUI.ViewModel;

namespace UpbeatUI.Extensions.DependencyInjection
{
    /// <summary>
    /// Represents a stack of ViewModels and provides methods and commands for controlling them. Uses an <see cref="IServiceProvider"/> to automatically resolve dependencies when creating ViewModels.
    /// </summary>
    public class ServiceProvidedUpbeatStack : UpbeatStack, IServiceProvidedUpbeatStack
    {
        private readonly Dictionary<Type, Func<IUpbeatService, object, object>> _childViewModelInstantiators = new Dictionary<Type, Func<IUpbeatService, object, object>>();
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<object, ViewModelScope> _viewModelParametersServiceScopes = new Dictionary<object, ViewModelScope>();
        private Action<Type> _autoMapper;

        /// <summary>
        /// Initializes an empty <see cref="ServiceProvidedUpbeatStack"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> that the <see cref="ServiceProvidedUpbeatStack"/> will use to resolve dependencies when instantiating ViewModels.</param>
        /// <param name="updateOnRender">True to have the <see cref="UpbeatStack"/> execute any UppdateCallback <see cref="Action"/>s registered by ViewModels on each WPF frame render; false to only execute UpdateCallback <see cref="Action"/>s manually.</param>
        public ServiceProvidedUpbeatStack(IServiceProvider serviceProvider, bool updateOnRender = true)
            : base(updateOnRender) =>
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        /// <inheritdoc/>
        public void MapViewModel<TParameters, TViewModel>(bool allowUnresolvedDependencies) =>
            MapServiceProvidedViewModel<TParameters, TViewModel>(allowUnresolvedDependencies);

        /// <inheritdoc/>
        public void MapViewModel<TParameters, TViewModel>(Func<IUpbeatService, TParameters, IServiceProvider, TViewModel> viewModelCreator) =>
            MapViewModel<TParameters, TViewModel>((upbeatService, parameters) =>
                viewModelCreator(
                    upbeatService,
                    parameters,
                    _viewModelParametersServiceScopes[parameters].ServiceProvider));

        /// <inheritdoc/>
        public override void OpenViewModel<TParameters>(TParameters parameters, Action closedCallback)
        {
            var parametersType = parameters.GetType();
            if (!ViewModelInstantiators.ContainsKey(parametersType))
            {
                _autoMapper?.Invoke(parametersType);
            }

            _viewModelParametersServiceScopes[parameters] = new ViewModelScope(_serviceProvider.CreateScope());
            base.OpenViewModel(parameters,
                () =>
                {
                    var viewModelScope = _viewModelParametersServiceScopes[parameters];
                    viewModelScope.Dispose();
                    _ = _viewModelParametersServiceScopes.Remove(viewModelScope);
                    closedCallback?.Invoke();
                });
        }

        /// <inheritdoc/>
        public void SetDefaultViewModelLocators(bool allowUnresolvedDependencies = false) =>
            SetViewModelLocators(
                (string parametersTypeString) => parametersTypeString
                    .Replace("+Parameters", "", StringComparison.Ordinal),
                allowUnresolvedDependencies);

        /// <inheritdoc/>
        public void SetViewModelLocators(
            Func<string, string> parameterToViewModelLocator,
            bool allowUnresolvedDependencies = false) =>
            SetViewModelLocators(
                (Type parametersType) => Type.GetType(parameterToViewModelLocator(parametersType.AssemblyQualifiedName)),
                allowUnresolvedDependencies);

        /// <inheritdoc/>
        public void SetViewModelLocators(
            Func<Type, Type> parameterToViewModelLocator,
            bool allowUnresolvedDependencies = false)
        {
            _ = parameterToViewModelLocator ?? throw new ArgumentNullException(nameof(parameterToViewModelLocator));
            _autoMapper = parametersType =>
            {
                var viewModelType = parameterToViewModelLocator(parametersType) ?? throw new InvalidOperationException($"Unable to locate ViewModel Type from Parameters Type: {parametersType.GetType().FullName}");
                var m = GetType().GetMethod(nameof(MapServiceProvidedViewModel), BindingFlags.NonPublic | BindingFlags.Instance);
                var n = m.MakeGenericMethod(parametersType, viewModelType);
                _ = n.Invoke(this, new object[] { allowUnresolvedDependencies });
            };
        }

        private Func<IUpbeatService, object, object> CreateInstantiator(
            ConstructorInfo constructor,
            Type targetType,
            bool allowUnresolvedDependencies) =>
            new Func<IUpbeatService, object, object>((upbeatService, parameters) => Convert.ChangeType(
                constructor.Invoke(constructor.GetParameters()
                    .Select(p =>
                    {
                        if (p.ParameterType == typeof(IUpbeatService))
                        {
                            return upbeatService;
                        }
                        if (p.ParameterType == parameters.GetType())
                        {
                            return parameters;
                        }
                        var viewModelScope = _viewModelParametersServiceScopes[parameters];
                        var locatedService = viewModelScope.ServiceProvider.GetService(p.ParameterType);
                        if (locatedService != null)
                        {
                            return locatedService;
                        }
                        if (viewModelScope.ChildViewModels.TryGetValue(p.ParameterType, out var childViewModel))
                        {
                            return childViewModel;
                        }
                        childViewModel = ResolveDependency(p.ParameterType, allowUnresolvedDependencies, upbeatService, parameters);
                        if (childViewModel == null && !allowUnresolvedDependencies)
                        {
                            throw new InvalidOperationException($"No service for type '{p.ParameterType.FullName}' has been registered.");
                        }
                        viewModelScope.ChildViewModels[p.ParameterType] = childViewModel;
                        return childViewModel;
                    })
                    .ToArray()),
                targetType,
                CultureInfo.InvariantCulture));

        private void MapServiceProvidedViewModel<TParameters, TViewModel>(bool allowUnresolvedDependencies)
        {
            var viewModelType = typeof(TViewModel);
            var constructors = viewModelType.GetConstructors().ToList();
            if (constructors.Count > 1)
            {
                throw new InvalidOperationException($"Type {viewModelType.FullName} has more than one constructor.");
            }

            var constructor = constructors[0];
            var instantiator = CreateInstantiator(constructor, viewModelType, allowUnresolvedDependencies);
            MapViewModel<TParameters, TViewModel>(
                (upbeatService, parameters) => (TViewModel)instantiator(upbeatService, parameters));
        }

        private object ResolveDependency(
            Type dependencyType,
            bool allowUnresolvedDependencies,
            IUpbeatService upbeatService,
            object parameters)
        {
            _ = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
            _ = parameters ?? throw new ArgumentNullException(nameof(parameters));
            if (!_childViewModelInstantiators.TryGetValue(dependencyType, out var instantiator))
            {
                var constructors = dependencyType.GetConstructors().ToList();
                if (constructors.Count > 1)
                {
                    return null;
                }

                var constructor = constructors[0];
                var parameterTypes = constructor.GetParameters().Select(p => p.ParameterType);
                if (!parameterTypes.Intersect(new[] { typeof(IUpbeatService), parameters.GetType() }).Any())
                {
                    return null;
                }

                instantiator = CreateInstantiator(constructor, dependencyType, allowUnresolvedDependencies);
                _childViewModelInstantiators[dependencyType] = instantiator;
            }
            return instantiator(upbeatService, parameters);
        }

        private sealed class ViewModelScope : IDisposable
        {
            private readonly IServiceScope _serviceScope;

            public ViewModelScope(IServiceScope serviceScope) =>
                _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));

            public Dictionary<Type, object> ChildViewModels { get; } = new Dictionary<Type, object>();
            public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

            public void Dispose()
            {
                foreach (var disposable in ChildViewModels.Values.OfType<IDisposable>().Reverse())
                {
                    disposable.Dispose();
                }

                _serviceScope.Dispose();
            }
        }
    }
}
