/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/pulselyre/upbeatui/blob/main/LICENSE.md
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using UpbeatUI.ViewModel;

namespace UpbeatUI.View.Converters
{
    /// <summary>
    /// Selects a <see cref="DataTemplate"/> for the View mapped to the bound ViewModel using the parent <see cref="UpbeatStack"/>.
    /// </summary>
    public class UpbeatViewSelector : DataTemplateSelector
    {
        private readonly IDictionary<Type, DataTemplate> _templateCache = new Dictionary<Type, DataTemplate>();
        private IUpbeatStack _upbeatStack;

        /// <summary>
        /// Returns a <see cref="DataTemplate"/> for the View mappened to the bound ViewModel using the parent <see cref="UpbeatStack"/>.
        /// </summary>
        /// <param name="item">The ViewModel object for which to select the View <see cref="DataTemplate"/>.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>A <see cref="DataTemplate"/> containg the View, or null if no appropriate View can be found.</returns>
        public override DataTemplate SelectTemplate(object item,
                                                    DependencyObject container)
        {
            if (_upbeatStack is null)
            {
                var element = (FrameworkElement)container;
                while (!(element.DataContext is IUpbeatStack))
                    element = (FrameworkElement)VisualTreeHelper.GetParent(element);
                _upbeatStack = (IUpbeatStack)element.DataContext;
            }
            var contextType = item?.GetType() ?? typeof(object);
            if (_templateCache.TryGetValue(contextType, out var dataTemplate))
                return dataTemplate;
            var controlType = _upbeatStack.GetViewTypeFromViewModelType(contextType);
            dataTemplate = controlType != null ? (DataTemplate)XamlReader.Parse(
                    $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:ns=\"clr-namespace:{controlType.Namespace};assembly={controlType.Assembly.FullName}\"><ns:{controlType.Name} /></DataTemplate>")
                : (DataTemplate)XamlReader.Parse(
                    $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Label Content=\"{contextType.GetType().FullName}\" /></DataTemplate>");
            _templateCache[contextType] = dataTemplate;
            return dataTemplate;
        }
    }
}
