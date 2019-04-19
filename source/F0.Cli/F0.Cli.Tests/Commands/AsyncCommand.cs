using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class AsyncCommand : CommandBase
	{
		public AsyncCommand()
		{
		}

		public override async Task<CommandResult> ExecuteAsync()
		{
			await Task.Yield();

			return Success();
		}
	}
}
