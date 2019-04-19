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
			CommandBase command = InternalCommand.CreateSuccess(null);

			CommandResult result = await command.ExecuteAsync();
			Assert.Equal(0, result.ExitCode);
		}

		[Fact]
		public async Task CanHaveError()
		{
			CommandBase command = InternalCommand.CreateError(null);

			CommandResult result = await command.ExecuteAsync();
			Assert.Equal(1, result.ExitCode);
		}

		[Fact]
		public void IsDisposable()
		{
			int count = 0;
			CommandBase command = InternalCommand.CreateSuccess(() => count++);

			Assert.Equal(0, count);
			command.Dispose();
			Assert.Equal(1, count);
		}
	}
}
