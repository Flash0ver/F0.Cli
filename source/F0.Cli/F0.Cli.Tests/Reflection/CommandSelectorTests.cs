using System;
using System.Collections.Generic;
using System.Reflection;
using F0.Cli;
using F0.Reflection;
using F0.Tests.Commands;
using Xunit;

namespace F0.Tests.Reflection
{
	public class CommandSelectorTests
	{
		[Fact]
		public void NullCheck()
		{
			Assert.Throws<ArgumentNullException>("assembly", () => CommandSelector.SelectCommand(null, CreateArgs("")));
			Assert.Throws<ArgumentNullException>("args", () => CommandSelector.SelectCommand(Assembly.GetEntryAssembly(), null));
		}

		[Fact]
		public void AssemblyWithoutAnyCommand()
		{
			Assert.Throws<CommandNotFoundException>(() => CommandSelector.SelectCommand(Assembly.GetCallingAssembly(), CreateArgs("null")));
		}

		[Fact]
		public void AssemblyWithCommands()
		{
			Type command = CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("null"));
			Assert.Equal(typeof(NullCommand), command);
		}

		[Fact]
		public void RequiresCommandToBeProvided()
		{
			Assert.Throws<CommandNotProvidedException>(() => CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("")));
		}

		[Fact]
		public void CannotMatchAmbiguousCommand()
		{
			Assert.Throws<AmbiguousCommandException>(() => CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("ambiguous")));
			Assert.Throws<AmbiguousCommandException>(() => CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("command")));
		}

		[Fact]
		public void MatchesOnlyCommands()
		{
			Assert.Throws<CommandNotFoundException>(() => CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs(nameof(CommandSelectorTests).ToLowerInvariant())));
		}

		[Fact]
		public void MatchesOnlyConcreteTypes()
		{
			Assert.Throws<CommandNotFoundException>(() => CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("abstract")));
		}

		[Fact]
		public void MatchesOnlyPublicTypes()
		{
			Assert.Throws<CommandNotFoundException>(() => CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("internal")));
		}

		[Fact]
		public void MatcherRemovesConventionalSuffix()
		{
			Type command = CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("delegate"));
			Assert.Equal(typeof(DelegateCommand), command);
		}

		[Fact]
		public void MatchesOnlyLowercase()
		{
			Assert.Throws<CommandNotFoundException>(() => CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("Null")));
			Assert.Throws<CommandNotFoundException>(() => CommandSelector.SelectCommand(Assembly.GetExecutingAssembly(), CreateArgs("delegatE")));
		}

		private static CommandLineArguments CreateArgs(string verb)
		{
			return new CommandLineArguments(verb, new List<string>(), new Dictionary<string, string>());
		}
	}
}
