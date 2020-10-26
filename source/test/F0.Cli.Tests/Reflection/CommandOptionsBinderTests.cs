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
			var command = new LogicalCommand();
			CommandLineArguments args = CreateArgs("switch", null);

			Assert.False(command.Switch);
			CommandOptionsBinder.BindOptions(command, args);
			Assert.True(command.Switch);
		}

		[Fact]
		public void OptionsWithValuesMayBeBoundToStringPropertiesWherePropertyNameMatchesOptionKey()
		{
			var command = new TextCommand();
			CommandLineArguments args = CreateArgs("text", "string");

			Assert.Null(command.Text);
			CommandOptionsBinder.BindOptions(command, args);
			Assert.Equal("string", command.Text);
		}

		[Fact]
		public void BooleanPropertiesAreBoundAgainstOptionsWithoutValues()
		{
			var command = new LogicalCommand();
			CommandLineArguments args = CreateArgs("switch", "bool");

			Assert.Throws<UnsupportedCommandOptionTypeException>(() => CommandOptionsBinder.BindOptions(command, args));
		}

		[Fact]
		public void StringPropertiesAreBoundAgainstOptionsWithValues()
		{
			var command = new TextCommand();
			CommandLineArguments args = CreateArgs("text", null);

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
