using System;
using System.Collections.Generic;
using F0.Cli;
using F0.Reflection;
using F0.Tests.Commands;
using Xunit;

namespace F0.Tests.Reflection
{
	public class CommandArgumentsBinderTests
	{
		[Fact]
		public void NullCheck()
		{
			var command = new NullCommand();
			CommandLineArguments args = CreateArgs();

			Assert.Throws<ArgumentNullException>("command", () => CommandArgumentsBinder.BindArguments(null, args));
			Assert.Throws<ArgumentNullException>("args", () => CommandArgumentsBinder.BindArguments(command, null));
		}

		[Fact]
		public void ArgumentsAreBoundToConventionallyNamedPropertyOfTypeStringArray()
		{
			var command = new TestCommand();
			CommandLineArguments args = CreateArgs("2", "4", "0");

			Assert.Null(command.Args);
			CommandArgumentsBinder.BindArguments(command, args);
			Assert.Equal(new string[] { "2", "4", "0" }, command.Args);
		}

		[Fact]
		public void ArgumentsAreBoundToConventionallyNamedPropertyOfTypeCollectionOfStrings()
		{
			var command = new Command();
			CommandLineArguments args = CreateArgs("2", "4", "0");

			Assert.Null(command.Arguments);
			CommandArgumentsBinder.BindArguments(command, args);
			Assert.Equal(new string[] { "2", "4", "0" }, command.Arguments);
		}

		[Fact]
		public void CommandRequiresMatchIfArgumentsAreProvided()
		{
			var command = new NullCommand();
			CommandLineArguments args = CreateArgs("F0");

			Assert.Throws<CommandArgumentsNotFoundException>(() => CommandArgumentsBinder.BindArguments(command, args));
		}

		[Fact]
		public void CommandCannotHaveMultipleConventionalMatchesForArguments()
		{
			var command = new AmbiguousCommand();
			CommandLineArguments args = CreateArgs("F0");

			Assert.Throws<AmbiguousCommandArgumentsException>(() => CommandArgumentsBinder.BindArguments(command, args));
		}

		[Fact]
		public void PropertyMustHaveCompatibleTypeToBeBoundAgainst()
		{
			var command = new InvalidCommand();
			CommandLineArguments args = CreateArgs("F0");

			Assert.Throws<UnsupportedCommandArgumentsTypeException>(() => CommandArgumentsBinder.BindArguments(command, args));
		}

		[Fact]
		public void CommandDoesNotRequireMatchIfNoArgumentsAreProvided()
		{
			var command = new NullCommand();
			CommandLineArguments args = CreateArgs();

			CommandArgumentsBinder.BindArguments(command, args);
		}

		private static CommandLineArguments CreateArgs(params string[] arguments)
		{
			return new CommandLineArguments(String.Empty, new List<string>(arguments), new Dictionary<string, string>());
		}
	}
}
