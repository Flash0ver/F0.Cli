using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public abstract class AbstractCommand : CommandBase
	{
		internal const string Name = "abstract";

		protected AbstractCommand()
		{
		}

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotSupportedException();
		}
	}
}
