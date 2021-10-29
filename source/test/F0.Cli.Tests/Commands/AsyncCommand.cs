using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class AsyncCommand : CommandBase
	{
		internal const string Name = "async";

		public AsyncCommand()
		{
		}

		public override async Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			await Task.Yield();

			return Success();
		}
	}
}
