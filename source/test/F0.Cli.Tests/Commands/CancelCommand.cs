using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class CancelCommand : CommandBase
	{
		public CancelCommand()
		{
		}

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				throw new InvalidOperationException("Cancellation has not been requested.");
			}

			return Task.FromCanceled<CommandResult>(cancellationToken);
		}
	}
}
