using System;
using System.Reflection;

namespace F0.Reflection
{
	internal sealed class CommandOptionBindingException : Exception
	{
		public CommandOptionBindingException(PropertyInfo property, string value, Exception inner)
			: base(CreateMessage(property, value, inner), inner)
		{
		}

		private static string CreateMessage(PropertyInfo property, string value, Exception inner)
		{
			string message = $"Cannot bind option '{property.Name}' with '{value}' to '{property.PropertyType}' of '{property.ReflectedType}': {inner.Message}";
			return message;
		}
	}
}
