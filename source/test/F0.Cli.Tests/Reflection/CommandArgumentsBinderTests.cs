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
			NullCommand command = new();
			CommandLineArguments args = CreateArgs();

			Assert.Throws<ArgumentNullException>("command", () => CommandArgumentsBinder.BindArguments(null!, args));
			Assert.Throws<ArgumentNullException>("args", () => CommandArgumentsBinder.BindArguments(command, null!));
		}

		[Fact]
		public void ArgumentsAreBoundToConventionallyNamedPropertyOfTypeStringArray()
		{
			TestCommand command = new();
			CommandLineArguments args = CreateArgs("2", "4", "0");

			Assert.Null(command.Args);
			CommandArgumentsBinder.BindArguments(command, args);
			Assert.Equal(new string[] { "2", "4", "0" }, command.Args);
		}

		[Fact]
		public void ArgumentsAreBoundToConventionallyNamedPropertyOfTypeCollectionOfStrings()
		{
			Command command = new();
			CommandLineArguments args = CreateArgs("2", "4", "0");

			Assert.Null(command.Arguments);
			CommandArgumentsBinder.BindArguments(command, args);
			Assert.NotNull(command.Arguments);
			Assert.Equal(new string[] { "2", "4", "0" }, command.Arguments);
		}

		[Fact]
		public void CommandRequiresMatchIfArgumentsAreProvided()
		{
			NullCommand command = new();
			CommandLineArguments args = CreateArgs("F0");

			Assert.Throws<CommandArgumentsNotFoundException>(() => CommandArgumentsBinder.BindArguments(command, args));
		}

		[Fact]
		public void CommandCannotHaveMultipleConventionalMatchesForArguments()
		{
			AmbiguousCommand command = new();
			CommandLineArguments args = CreateArgs("F0");

			Assert.Throws<AmbiguousCommandArgumentsException>(() => CommandArgumentsBinder.BindArguments(command, args));
		}

		[Fact]
		public void PropertyMustHaveCompatibleTypeToBeBoundAgainst()
		{
			InvalidCommand command = new();
			CommandLineArguments args = CreateArgs("F0");

			Assert.Throws<UnsupportedCommandArgumentsTypeException>(() => CommandArgumentsBinder.BindArguments(command, args));
		}

		[Fact]
		public void CommandDoesNotRequireMatchIfNoArgumentsAreProvided()
		{
			NullCommand command = new();
			CommandLineArguments args = CreateArgs();

			CommandArgumentsBinder.BindArguments(command, args);
		}

		[Fact]
		public void BindingRequiresPublicSetAccessor_MustNotBeNonPublic()
		{
			InternalCommand command = new();
			CommandLineArguments args = CreateArgs("internal");

			Assert.Throws<CommandArgumentsNotFoundException>(() => CommandArgumentsBinder.BindArguments(command, args));
		}

		[Fact]
		public void BindingRequiresSetAccessor_MustNotBeReadOnly()
		{
			ReadOnlyCommand command = new();
			CommandLineArguments args = CreateArgs("read-only");

			Assert.Throws<ReadOnlyCommandArgumentsException>(() => CommandArgumentsBinder.BindArguments(command, args));
		}

		[Fact]
		public void SupportsInitOnlySetters()
		{
			InitOnlyCommand command = new();
			CommandLineArguments args = CreateArgs("init-only setters");

			Assert.Null(command.Args);
			CommandArgumentsBinder.BindArguments(command, args);
			Assert.Equal(new string[] { "init-only setters" }, command.Args);
		}

		private static CommandLineArguments CreateArgs(params string[] arguments)
		{
			return new CommandLineArguments(String.Empty, new List<string>(arguments), new Dictionary<string, string?>());
		}
	}
}
