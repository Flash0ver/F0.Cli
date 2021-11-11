using System;
using System.Collections.Generic;
using F0.Cli;
using F0.Reflection;
using F0.Tests.Commands;
using Xunit;

namespace F0.Tests.Reflection
{
	public partial class CommandOptionsBinderTests
	{
		[Fact]
		public void NullCheck()
		{
			NullCommand command = new();
			CommandLineArguments args = CreateArgs();

			Assert.Throws<ArgumentNullException>("command", () => CommandOptionsBinder.BindOptions(null!, args));
			Assert.Throws<ArgumentNullException>("args", () => CommandOptionsBinder.BindOptions(command, null!));
		}

		[Fact]
		public void OptionsWithoutValuesAreBoundToBooleanPropertiesWherePropertyNameMatchesOptionKey()
		{
			LogicalCommand command = new();
			CommandLineArguments args = CreateArgs(("switch", null));

			Assert.False(command.Switch);
			CommandOptionsBinder.BindOptions(command, args);
			Assert.True(command.Switch);
		}

		[Fact]
		public void OptionsWithValuesMayBeBoundToStringPropertiesWherePropertyNameMatchesOptionKey()
		{
			TextCommand command = new();
			CommandLineArguments args = CreateArgs(("text", "string"));

			Assert.Null(command.Text);
			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal("string", command.Text);
		}

		[Fact]
		public void BooleanPropertiesAreBoundAgainstOptionsWithoutValues()
		{
			LogicalCommand command = new();
			CommandLineArguments args = CreateArgs(("switch", "bool"));

			Assert.Throws<UnsupportedCommandOptionTypeException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void StringPropertiesAreBoundAgainstOptionsWithValues()
		{
			TextCommand command = new();
			CommandLineArguments args = CreateArgs(("text", null));

			Assert.Throws<InvalidCommandSwitchException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void OptionKeysMustMatchExactly()
		{
			Command command = new();
			CommandLineArguments args = CreateArgs(("option0", "F0"));

			Assert.Throws<CommandOptionNotFoundException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void OptionKeysMustBeLowercase()
		{
			Command command = new();
			CommandLineArguments args = CreateArgs(("Option", "240"));

			Assert.Throws<CommandOptionNotFoundException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void OptionsAreOptional()
		{
			TestCommand command = new();
			CommandLineArguments args = CreateArgs();

			bool option1 = command.Option1;
			string? option2 = command.Option2;
			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal(option1, command.Option1);
			Assert.Same(option2, command.Option2);
		}

		[Fact]
		public void BindingRequiresPublicSetAccessor_MustNotBeNonPublic()
		{
			InternalCommand command = new();
			CommandLineArguments args = CreateArgs(("internal", "value"));

			Assert.Throws<CommandOptionNotFoundException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void BindingRequiresSetAccessor_MustNotBeReadOnly()
		{
			ReadOnlyCommand command = new();
			CommandLineArguments args = CreateArgs(("readonly", "value"));

			Assert.Throws<ReadOnlyCommandOptionException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void SupportsInitOnlySetters()
		{
			InitOnlyCommand command = new();
			CommandLineArguments args = CreateArgs(("initonly", "C# 9.0 or greater"));

			Assert.Null(command.InitOnly);
			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal("C# 9.0 or greater", command.InitOnly);
		}

		private static CommandLineArguments CreateArgs(params (string Key, string? Value)[] options)
		{
			Dictionary<string, string?> switches = new();

			foreach ((string Key, string? Value) option in options)
			{
				switches.Add(option.Key, option.Value);
			}

			return new CommandLineArguments(String.Empty, new List<string>(), switches);
		}
	}
}
