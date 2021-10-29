using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class ExceptionCommand : CommandBase
	{
		internal const string Name = "exception";

		public ExceptionCommand()
		{
		}

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			return Task.FromException<CommandResult>(new CommandException());
		}
	}

	internal sealed class CommandException : Exception
	{
		public CommandException()
			: base(CreateMessage())
		{
		}

		private static string CreateMessage()
		{
			string message = "An exceptional situation has occurred.";
			return message;
		}
	}
}
