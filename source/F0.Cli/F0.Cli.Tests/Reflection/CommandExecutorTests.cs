using System;
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
			await Assert.ThrowsAsync<ArgumentNullException>("command", () => CommandExecutor.InvokeAsync(null));
		}

		[Fact]
		public async Task ExecuteCommandSuccessfully()
		{
			var command = new DelegateCommand(() => 0x_F0);

			CommandResult result = await CommandExecutor.InvokeAsync(command);
			Assert.Equal(0x_F0, result.ExitCode);
		}

		[Fact]
		public async Task ExecuteCommandWithAnUnhandledException_AsWrappedException()
		{
			var command = new ErrorCommand();

			await Assert.ThrowsAsync<CommandExecutionException>(() => CommandExecutor.InvokeAsync(command));
		}
	}
}
