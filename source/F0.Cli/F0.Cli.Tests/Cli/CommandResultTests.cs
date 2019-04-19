using F0.Cli;
using Xunit;

namespace F0.Tests.Cli
{
	public class CommandResultTests
	{
		[Fact]
		public void HasExitCode()
		{
			var result = new CommandResult(0xF0);
			Assert.Equal(240, result.ExitCode);
		}

		[Fact]
		public void ZeroIndicatesThatTheProcessCompletedSuccessfully()
		{
			var result = new CommandResult(0);
			Assert.True(result.IsSuccess);
			Assert.False(result.IsFailure);
		}

		[Fact]
		public void NonZeroNumberIndicatesAnError()
		{
			var result = new CommandResult(0xF0);
			Assert.False(result.IsSuccess);
			Assert.True(result.IsFailure);
		}

		[Fact]
		public void DefaultResultHasExitCodeValueOfZero()
		{
			var defaultResult = new CommandResult();
			Assert.Equal(0, defaultResult.ExitCode);
		}
	}
}
