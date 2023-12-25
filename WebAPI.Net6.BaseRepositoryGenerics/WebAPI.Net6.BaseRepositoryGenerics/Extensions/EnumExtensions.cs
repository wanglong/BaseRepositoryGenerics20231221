using System.ComponentModel;
using System.Reflection;

namespace WebAPI.Net6.BaseRepositoryGenerics.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString<TEnum>(this TEnum @enum)
        {
            if (@enum == null)
                return String.Empty;

            FieldInfo? info = @enum.GetType().GetField(@enum.ToString() ?? string.Empty);
            if (info == null)
                return String.Empty;

            var attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes[0].Description ?? @enum.ToString() ?? string.Empty;
        }
    }
}
