using System;
using System.Reflection;

namespace F0.Reflection
{
	internal sealed class ReadOnlyCommandOptionException : Exception
	{
		public ReadOnlyCommandOptionException(PropertyInfo property)
			: base(CreateMessage(property))
		{
		}

		private static string CreateMessage(PropertyInfo property)
		{
			string message = $"Cannot bind a value for property '{property.Name}' of type '{property.PropertyType}' on command type '{property.ReflectedType}' because the property is read-only.";
			return message;
		}
	}
}
