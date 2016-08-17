using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace GitClientVS.UI.Extensions
{
    public class EnumToItemsSource : MarkupExtension
    {
        private readonly Type _type;
        private readonly string _defaultLabel;

        public EnumToItemsSource(Type type, string defaultLabel = null)
        {
            _type = type;
            _defaultLabel = defaultLabel;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(_type).Cast<object>().ToList();
            enumValues.Insert(0, null);
            return enumValues.Select(e => new { Value = e, DisplayName = e?.ToString() ?? _defaultLabel });
        }
    }
}