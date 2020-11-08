/* This file is part of the UpbeatUI project, which is released under MIT License.
 * See LICENSE.md or visit:
 * https://github.com/michaelpduda/upbeatui/blob/master/LICENSE.md
 */
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using UpbeatUI.ViewModel;

namespace UpbeatUI.View.Converters
{
    [Obsolete("This converter is no longer used by the the standard UpbeatControl implementation, and will be removed in UpbeatUI 3.0.")]
    public class MappingsToSelectorConverter : ValueConverterMarkupExtension<MappingsToSelectorConverter>
    {
        public override object Convert(object value,
                                       Type targetType,
                                       object parameter,
                                       CultureInfo culture) =>
            new ControlMappingSelector((UpbeatStack)(value ?? throw new ArgumentNullException(nameof(parameter))));

        private class ControlMappingSelector : DataTemplateSelector
        {
            private UpbeatStack _upbeatStack;

            public ControlMappingSelector(UpbeatStack upbeatStack) =>
                _upbeatStack = upbeatStack ?? throw new ArgumentNullException(nameof(upbeatStack));

            public override DataTemplate SelectTemplate(object item,
                                                        DependencyObject container)
            {
                var contextType = item?.GetType() ?? typeof(object);
                var controlType = _upbeatStack.GetViewTypeFromViewModelType(contextType);
                return
                    controlType != null ? (DataTemplate)XamlReader.Parse(
                        $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:ns=\"clr-namespace:{controlType.Namespace};assembly={controlType.Assembly.FullName}\"><ns:{controlType.Name} /></DataTemplate>")
                    : (DataTemplate)XamlReader.Parse(
                        $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Label Content=\"{contextType.GetType().Name}\" /></DataTemplate>");
            }
        }
    }
}
