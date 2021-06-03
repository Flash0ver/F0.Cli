using F0.Cli;
using Xunit;

namespace F0.Tests.Cli
{
	public class CommandResultTests
	{
		[Fact]
		public void HasExitCode()
		{
			CommandResult result = new(0xF0);
			Assert.Equal(240, result.ExitCode);
		}

		[Fact]
		public void ZeroIndicatesThatTheProcessCompletedSuccessfully()
		{
			CommandResult result = new(0);
			Assert.True(result.IsSuccess);
			Assert.False(result.IsFailure);
		}

		[Fact]
		public void NonZeroNumberIndicatesAnError()
		{
			CommandResult result = new(0xF0);
			Assert.False(result.IsSuccess);
			Assert.True(result.IsFailure);
		}

		[Fact]
		public void DefaultResultHasExitCodeValueOfZero()
		{
			CommandResult defaultResult = new();
			Assert.Equal(0, defaultResult.ExitCode);
		}
	}
}
