using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;
using F0.Tests.Commands;
using Xunit;

namespace F0.Tests.Cli
{
	public class CommandBaseTests
	{
		[Fact]
		public async Task CanCompleteSuccessfully()
		{
			CommandBase command = new SuccessCommand();

			CommandResult result = await command.ExecuteAsync(CancellationToken.None);
			Assert.Equal(0, result.ExitCode);
		}

		[Fact]
		public async Task CanHaveError()
		{
			CommandBase command = new ErrorCommand();

			CommandResult result = await command.ExecuteAsync(CancellationToken.None);
			Assert.Equal(1, result.ExitCode);
		}

		[Fact]
		public async Task PropagatesAnyException()
		{
			CommandBase command = new ExceptionCommand();

			await Assert.ThrowsAsync<CommandException>(() => command.ExecuteAsync(CancellationToken.None));
		}

		[Fact]
		public async Task SupportsCooperativeCancellation()
		{
			using (var cts = new CancellationTokenSource())
			{
				CommandBase command = new CancellationCommand();
				cts.Cancel();

				OperationCanceledException exception = await Assert.ThrowsAsync<OperationCanceledException>(() => command.ExecuteAsync(cts.Token));
				Assert.Equal(cts.Token, exception.CancellationToken);
			}

			using (var cts = new CancellationTokenSource())
			{
				CommandBase command = new CancelCommand();
				cts.Cancel();

				Task<CommandResult> task = command.ExecuteAsync(cts.Token);

				TaskCanceledException exception = await Assert.ThrowsAsync<TaskCanceledException>(() => task);
				Assert.Equal(cts.Token, exception.CancellationToken);
				Assert.Same(task, exception.Task);

				Assert.Equal(TaskStatus.Canceled, task.Status);
				Assert.Null(task.Exception);
			}
		}

		[Fact]
		public void IsDisposable()
		{
			int count = 0;
			CommandBase command = new DisposeCommand(() => count++);

			Assert.Equal(0, count);
			command.Dispose();
			Assert.Equal(1, count);
		}
	}
}
