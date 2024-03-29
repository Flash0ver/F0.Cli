using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	internal sealed class InternalCommand : CommandBase
	{
		internal const string Name = "internal";

		public InternalCommand()
		{
		}

		internal string[]? Args { get; set; }

		internal string? Internal { get; set; }

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
