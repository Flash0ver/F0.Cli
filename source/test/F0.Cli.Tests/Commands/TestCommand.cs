using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class TestCommand : CommandBase
	{
		public TestCommand()
		{
		}

		public string[]? Args { get; set; }

		public bool Option1 { get; set; }
		public string? Option2 { get; set; }

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotSupportedException();
		}
	}
}
