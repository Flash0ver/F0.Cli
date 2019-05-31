using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class LongRunningCommand : CommandBase
	{
		public LongRunningCommand()
		{
		}

		public override async Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			var tcs = new TaskCompletionSource<object>();

			CancellationTokenRegistration ctr = cancellationToken.Register(state =>
			{
				((TaskCompletionSource<object>)state).TrySetResult(null);
			}, tcs);

			await tcs.Task;

			ctr.Dispose();

			return Success();
		}
	}
}
