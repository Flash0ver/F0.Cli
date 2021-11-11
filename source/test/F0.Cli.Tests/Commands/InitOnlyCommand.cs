using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class InitOnlyCommand : CommandBase
	{
		public InitOnlyCommand()
		{
		}

		public string[]? Args { get; init; }

		public string? InitOnly { get; init; }

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
