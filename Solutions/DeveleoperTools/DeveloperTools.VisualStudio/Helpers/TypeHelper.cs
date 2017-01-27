using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPnP.DeveloperTools.VisualStudio.Helpers
{
	public static class TypeHelper
	{
		public static T GetPropertyValue<T>(this object source, string propertyName)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}

			object value = null;
			System.ComponentModel.PropertyDescriptor property = System.ComponentModel.TypeDescriptor.GetProperties(source)[propertyName];
			if (property != null)
			{
				value = property.GetValue(source);
			}
			return value != null ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture) : default(T);
		}
	}
}
