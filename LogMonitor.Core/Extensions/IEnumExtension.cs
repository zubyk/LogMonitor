using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LogMonitor
{
    public static class EnumExtension
    {
        public static string GetDisplayName<T>(this T value) where T : Enum, IComparable, IFormattable, IConvertible
        {
            var valueText = value.ToString();

            var displayAttribute = value.GetType()
                .GetMember(valueText)
                .First()
                .GetCustomAttribute<DisplayAttribute>();

            string? displayName = displayAttribute?.GetName();

            return displayName ?? valueText;
        }
    }
}
