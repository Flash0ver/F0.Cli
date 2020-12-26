using System;
using F0.Cli;

namespace F0.Reflection
{
	internal sealed class CommandArgumentsNotFoundException : Exception
	{
		public CommandArgumentsNotFoundException(CommandBase command)
			: base(CreateMessage(command))
		{
		}

		private static string CreateMessage(CommandBase command)
		{
			Type type = command.GetType();
			string message = $"No bindable Arguments property defined by the Command type '{type}'.";
			return message;
		}
	}
}
