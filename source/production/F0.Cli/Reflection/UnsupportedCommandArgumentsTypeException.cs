using System;
using System.Reflection;

namespace F0.Reflection
{
	internal sealed class UnsupportedCommandArgumentsTypeException : Exception
	{
		public UnsupportedCommandArgumentsTypeException(PropertyInfo property)
			: base(CreateMessage(property))
		{
		}

		private static string CreateMessage(PropertyInfo property)
		{
			string message = $"'{property.Name}' property of '{property.ReflectedType}' of type '{property.PropertyType}' not supported.";
			return message;
		}
	}
}
