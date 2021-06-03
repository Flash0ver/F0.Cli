using System;
using System.Collections.Generic;
using F0.Cli;
using Xunit;

namespace F0.Tests.Cli
{
	public class CommandLineArgumentsTests
	{
		[Fact]
		public void ParametersMustNotBeNull()
		{
			string verb = String.Empty;
			IReadOnlyList<string> arguments = CreateArguments();
			IReadOnlyDictionary<string, string> options = CreateOptions();

			Assert.Throws<ArgumentNullException>(nameof(verb), () => new CommandLineArguments(null, arguments, options));
			Assert.Throws<ArgumentNullException>(nameof(arguments), () => new CommandLineArguments(verb, null, options));
			Assert.Throws<ArgumentNullException>(nameof(options), () => new CommandLineArguments(verb, arguments, null));
		}

		[Fact]
		public void WhenInputHasVerb_ThenInputIsCommand()
		{
			CommandLineArguments args = new("verb", CreateArguments(), CreateOptions());

			Assert.Equal("verb", args.Verb);

			Assert.False(args.IsDriver);
			Assert.True(args.IsCommand);
		}

		[Fact]
		public void WhenInputHasNoVerb_ThenInputIsNoCommand()
		{
			CommandLineArguments args = new("", CreateArguments(), CreateOptions());

			Assert.Empty(args.Verb);

			Assert.True(args.IsDriver);
			Assert.False(args.IsCommand);
		}

		[Fact]
		public void OptionalArgumentsAreCollectionOfStrings()
		{
			CommandLineArguments args = new("", CreateArguments("a", "b", "c"), CreateOptions());

			Assert.Equal(new List<string> { "a", "b", "c" }, args.Arguments);
			Assert.True(args.HasArguments);

			Assert.Empty(args.Options);
			Assert.False(args.HasOptions);
		}

		[Fact]
		public void OptionalOptionsAreKeyValuePairsOfStrings()
		{
			CommandLineArguments args = new("", CreateArguments(), CreateOptions("a", "1", "b", "2", "c", "3"));

			Assert.Empty(args.Arguments);
			Assert.False(args.HasArguments);

			Assert.Equal(new Dictionary<string, string> { { "a", "1" }, { "b", "2" }, { "c", "3" } }, args.Options);
			Assert.True(args.HasOptions);
		}

		private static IReadOnlyList<string> CreateArguments(params string[] arguments)
		{
			return new List<string>(arguments);
		}

		private static IReadOnlyDictionary<string, string> CreateOptions(params string[] options)
		{
			Dictionary<string, string> switches = new();

			for (int i = 0; i < options.Length; i += 2)
			{
				string key = options[i];
				string value = options[i + 1];
				switches.Add(key, value);
			}

			return switches;
		}
	}
}
