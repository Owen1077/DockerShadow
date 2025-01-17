using System.Collections.Specialized;
using System.ComponentModel;

namespace DockerShadow.Core.Extension
{
    public static class Extensions
    {
        public static NameValueCollection ToNameValueCollection<T>(this T dynamicObject)
        {
            var nameValueCollection = new NameValueCollection();
            if (dynamicObject is null)
            {
                return nameValueCollection;
            }
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynamicObject))
            {
                string? value = propertyDescriptor.GetValue(dynamicObject)?.ToString();
                nameValueCollection.Add(propertyDescriptor.Name, value);
            }
            return nameValueCollection;
        }

        public static IDictionary<string, string?> ToDictionary<T>(this T dynamicObject)
        {
            var dict = new Dictionary<string, string?>();

            if (dynamicObject != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynamicObject))
                {
                    string? value = propertyDescriptor.GetValue(dynamicObject)?.ToString();
                    dict.Add(propertyDescriptor.Name, value);
                }
            }

            return dict;
        }

        public static decimal ConvertStroopsToRealValue(this long request)
        {
            return (decimal)(request * Math.Pow(10, -7));
        }
    }
}
