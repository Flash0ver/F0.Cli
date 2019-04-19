using System;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class DelegateCommand : CommandBase
	{
		private readonly Func<int> onExecute;

		public DelegateCommand(Func<int> onExecute)
		{
			this.onExecute = onExecute;
		}

		public override Task<CommandResult> ExecuteAsync()
		{
			int exitCode = onExecute();

			return Task.FromResult(new CommandResult(exitCode));
		}
	}
}
