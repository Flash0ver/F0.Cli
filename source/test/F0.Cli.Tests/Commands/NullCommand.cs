using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class NullCommand : CommandBase
	{
		internal const string Name = "null";

		public NullCommand()
		{
		}

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(Success());
		}
	}
}
