using System;

namespace F0.Reflection
{
	internal sealed class CommandNotProvidedException : Exception
	{
		public CommandNotProvidedException()
			: base(CreateMessage())
		{
		}

		private static string CreateMessage()
		{
			string message = $"Required command was not provided.";
			return message;
		}
	}
}
