using System;
using F0.Cli;

namespace F0.Reflection
{
	internal sealed class CommandCanceledException : Exception
	{
		public CommandCanceledException(CommandBase command, Exception inner)
			: base(CreateMessage(command), inner)
		{
		}

		private static string CreateMessage(CommandBase command)
		{
			string commandName = command.GetType().Name;
			string message = $"The command '{commandName}' was canceled.";
			return message;
		}
	}
}
