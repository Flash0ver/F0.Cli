using System;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class ExitCodeCommand : CommandBase
	{
		public ExitCodeCommand()
		{
		}

		public string ExitCode { get; set; }

		public override Task<CommandResult> ExecuteAsync()
		{
			if (ExitCode is null)
			{
				return Task.FromResult(Success());
			}
			else
			{
				int exitCode = Int32.Parse(ExitCode);
				return Task.FromResult(new CommandResult(exitCode));
			}
		}
	}
}
