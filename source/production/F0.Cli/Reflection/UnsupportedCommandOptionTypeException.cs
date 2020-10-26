using System;
using System.Reflection;

namespace F0.Reflection
{
	internal sealed class UnsupportedCommandOptionTypeException : Exception
	{
		public UnsupportedCommandOptionTypeException(PropertyInfo property)
			: base(CreateMessage(property))
		{
		}

		private static string CreateMessage(PropertyInfo property)
		{
			string message = $"'{property.Name}' of '{property.ReflectedType}' is not a valid option of type '{property.PropertyType}'.";
			return message;
		}
	}
}
