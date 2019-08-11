using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class InvalidCommand : CommandBase
	{
		internal InvalidCommand()
		{
		}

		public int Args { get; set; }

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotSupportedException();
		}
	}
}
