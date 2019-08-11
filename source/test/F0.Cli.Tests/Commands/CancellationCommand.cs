using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class CancellationCommand : CommandBase
	{
		public CancellationCommand()
		{
		}

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			throw new InvalidOperationException("Cancellation has not been requested.");
		}
	}
}
