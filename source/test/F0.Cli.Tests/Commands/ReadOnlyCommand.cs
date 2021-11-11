using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class ReadOnlyCommand : CommandBase
	{
		public ReadOnlyCommand()
		{
			Args = Array.Empty<string>();
			ReadOnly = String.Empty;
		}

		public string[] Args { get; }

		public string ReadOnly { get; }

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
