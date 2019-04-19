using System;
using System.Reflection;

namespace F0.Reflection
{
	internal sealed class InvalidCommandSwitchException : Exception
	{
		public InvalidCommandSwitchException(PropertyInfo property)
			 : base(CreateMessage(property))
		{
		}

		private static string CreateMessage(PropertyInfo property)
		{
			string message = $"'{property.Name}' of '{property.ReflectedType}' is not a valid switch of type {nameof(Boolean)}.";
			return message;
		}
	}
}
