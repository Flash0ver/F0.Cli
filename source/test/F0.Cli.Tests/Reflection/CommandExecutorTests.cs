using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;
using F0.Reflection;
using F0.Tests.Commands;
using Xunit;

namespace F0.Tests.Reflection
{
	public class CommandExecutorTests
	{
		[Fact]
		public async Task NullCheck()
		{
			await Assert.ThrowsAsync<ArgumentNullException>("command", () => CommandExecutor.InvokeAsync(null, CancellationToken.None));
		}

		[Fact]
		public async Task ExecuteCommandSuccessfully()
		{
			var command = new DelegateCommand(() => 0x_F0);

			CommandResult result = await CommandExecutor.InvokeAsync(command, CancellationToken.None);
			Assert.Equal(0x_F0, result.ExitCode);
		}

		[Fact]
		public async Task ExecuteCancelableCommand_ThrowsWrappedException()
		{
			using (var cts = new CancellationTokenSource())
			{
				var command = new CancellationCommand();
				cts.Cancel();

				CommandCanceledException exception = await Assert.ThrowsAsync<CommandCanceledException>(() => CommandExecutor.InvokeAsync(command, cts.Token));
				OperationCanceledException inner = Assert.IsType<OperationCanceledException>(exception.InnerException);

				Assert.Equal(cts.Token, inner.CancellationToken);
			}

			using (var cts = new CancellationTokenSource())
			{
				var command = new CancelCommand();
				cts.Cancel();

				Task<CommandResult> task = CommandExecutor.InvokeAsync(command, cts.Token);

				CommandCanceledException exception = await Assert.ThrowsAsync<CommandCanceledException>(() => task);
				TaskCanceledException inner = Assert.IsType<TaskCanceledException>(exception.InnerException);

				Assert.Equal(cts.Token, inner.CancellationToken);
				Assert.NotSame(task, inner.Task);
				Assert.Equal(TaskStatus.Faulted, task.Status);
				Assert.Same(exception, task.Exception.InnerExceptions.Single());
			}
		}

		[Fact]
		public async Task ExecuteCommandWithAnUnhandledException_ThrowsWrappedException()
		{
			var command = new ExceptionCommand();

			CommandExecutionException exception = await Assert.ThrowsAsync<CommandExecutionException>(() => CommandExecutor.InvokeAsync(command, CancellationToken.None));
			Assert.IsType<CommandException>(exception.InnerException);
		}
	}
}
