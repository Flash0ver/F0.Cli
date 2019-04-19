using System;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class ErrorCommand : CommandBase
	{
		public ErrorCommand()
		{
		}

		public int Args { get; set; }

		public override async Task<CommandResult> ExecuteAsync()
		{
			await Task.Yield();

			throw new InvalidOperationException();
		}
	}
}
