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

#if !NETCOREAPP2_1
			await
#endif
			using CancellationTokenRegistration ctr = cancellationToken.Register(state =>
			{
				_ = ((TaskCompletionSource<object>)state).TrySetResult(null);
			}, tcs);

			_ = await tcs.Task;

			return Success();
		}
	}
}
