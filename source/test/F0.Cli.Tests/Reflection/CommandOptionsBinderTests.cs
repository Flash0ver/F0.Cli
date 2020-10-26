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
			var command = new NullCommand();
			CommandLineArguments args = CreateArgs();

			Assert.Throws<ArgumentNullException>("command", () => CommandOptionsBinder.BindOptions(null, args));
			Assert.Throws<ArgumentNullException>("args", () => CommandOptionsBinder.BindOptions(command, null));
		}

		[Fact]
		public void OptionsWithoutValuesAreBoundToBooleanPropertiesWherePropertyNameMatchesOptionKey()
		{
			var command = new TestCommand();
			CommandLineArguments args = CreateArgs("option1", null);

			CommandOptionsBinder.BindOptions(command, args);
			Assert.True(command.Option1);
			Assert.Null(command.Option2);
		}

		[Fact]
		public void OptionsWithValuesAreBoundToStringPropertiesWherePropertyNameMatchesOptionKey()
		{
			var command = new TestCommand();
			CommandLineArguments args = CreateArgs("option2", "string");

			CommandOptionsBinder.BindOptions(command, args);
			Assert.False(command.Option1);
			Assert.Equal("string", command.Option2);
		}

		[Fact]
		public void BooleanPropertiesAreBoundAgainstOptionsWithoutValues()
		{
			var command = new TestCommand();
			CommandLineArguments args = CreateArgs("option1", "bool");

			Assert.Throws<UnsupportedCommandOptionTypeException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void StringPropertiesAreBoundAgainstOptionsWithValues()
		{
			var command = new TestCommand();
			CommandLineArguments args = CreateArgs("option2", null);

			Assert.Throws<InvalidCommandSwitchException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void OptionKeysMustMatchExactly()
		{
			var command = new Command();
			CommandLineArguments args = CreateArgs("option0", "F0");

			Assert.Throws<CommandOptionNotFoundException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void OptionKeysMustBeLowercase()
		{
			var command = new Command();
			CommandLineArguments args = CreateArgs("Option", "240");

			Assert.Throws<CommandOptionNotFoundException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void OptionsAreOptional()
		{
			var command = new TestCommand();
			CommandLineArguments args = CreateArgs();

			bool option1 = command.Option1;
			string option2 = command.Option2;
			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal(option1, command.Option1);
			Assert.Same(option2, command.Option2);
		}

		private static CommandLineArguments CreateArgs(params string[] options)
		{
			var switches = new Dictionary<string, string>();

			for (int i = 0; i < options.Length; i += 2)
			{
				string key = options[i];
				string value = options[i + 1];
				switches.Add(key, value);
			}

			return new CommandLineArguments(String.Empty, new List<string>(), switches);
		}
	}
}
