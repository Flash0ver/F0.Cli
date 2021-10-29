using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class TextCommand : CommandBase
	{
		public TextCommand()
		{
		}

		public string? Text { get; set; }

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
