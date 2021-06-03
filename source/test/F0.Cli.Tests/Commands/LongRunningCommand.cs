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
			TaskCompletionSource<object> tcs = new();

#if HAS_ASYNCHRONOUS_DISPOSABLE
			await
#endif
			using CancellationTokenRegistration ctr = cancellationToken.Register(static state =>
			{
				_ = ((TaskCompletionSource<object>)state).TrySetResult(null);
			}, tcs);

			_ = await tcs.Task;

			return Success();
		}
	}
}
