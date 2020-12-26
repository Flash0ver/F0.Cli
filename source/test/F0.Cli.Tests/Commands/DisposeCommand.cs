using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class DisposeCommand : CommandBase
	{
		private readonly Action onDispose;

		public DisposeCommand(Action onDispose)
		{
			this.onDispose = onDispose;
		}

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotSupportedException();
		}

		public override void Dispose()
		{
			onDispose();
		}
	}
}
