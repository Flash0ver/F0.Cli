using System;

namespace F0.Reflection
{
	internal sealed class CommandNotFoundException : Exception
	{
		public CommandNotFoundException(string verb)
			: base(CreateMessage(verb))
		{
		}

		private static string CreateMessage(string verb)
		{
			string message = $"Command '{verb}' not found.";
			return message;
		}
	}
}
