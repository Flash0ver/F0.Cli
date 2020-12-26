using System;
using F0.Cli;

namespace F0.Reflection
{
	internal sealed class CommandExecutionException : Exception
	{
		public CommandExecutionException(CommandBase command, Exception inner)
			: base(CreateMessage(command), inner)
		{
		}

		private static string CreateMessage(CommandBase command)
		{
			string commandName = command.GetType().Name;
			string message = $"An error occurred while executing the command '{commandName}'. See {nameof(InnerException)} for details.";
			return message;
		}
	}
}
