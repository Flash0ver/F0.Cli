using System;
using F0.Cli;

namespace F0.Reflection
{
	internal sealed class CommandOptionNotFoundException : Exception
	{
		public CommandOptionNotFoundException(CommandBase command, string option)
			: base(CreateMessage(command, option))
		{
		}

		private static string CreateMessage(CommandBase command, string option)
		{
			Type type = command.GetType();
			string message = $"Bindable Option '{option}' not defined by the Command type '{type}'.";
			return message;
		}
	}
}
