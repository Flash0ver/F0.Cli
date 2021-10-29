using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class AmbiguousCommand : CommandBase
	{
		public AmbiguousCommand()
		{
		}

		public string[]? Args { get; set; }
		public IEnumerable<string>? Arguments { get; set; }

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotSupportedException();
		}
	}
}
