using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class LongRunningCommand : CommandBase
	{
		internal const string Name = "longrunning";

		public LongRunningCommand()
		{
		}

		public override async Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			TaskCompletionSource tcs = new();

#if HAS_ASYNCHRONOUS_DISPOSABLE
			await
#endif
			using CancellationTokenRegistration ctr = cancellationToken.Register(static state =>
			{
				Debug.Assert(state is TaskCompletionSource);
				_ = ((TaskCompletionSource)state).TrySetResult();
			}, tcs);

			await tcs.Task;

			return Success();
		}
	}
}
