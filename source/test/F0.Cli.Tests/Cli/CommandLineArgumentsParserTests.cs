using System;
using System.Collections.Generic;
using F0.Cli;
using Xunit;

namespace F0.Tests.Cli
{
	public class CommandLineArgumentsParserTests
	{
		[Fact]
		public void CannotParseNull()
		{
			Assert.Throws<ArgumentNullException>("args", () => CommandLineArgumentsParser.Parse(null));
		}

		[Fact]
		public void CannotParseAnonymousOptions()
		{
			Assert.Throws<AnonymousOptionException>(() => Parse("--"));
			Assert.Throws<AnonymousOptionException>(() => Parse("-"));
		}

		[Fact]
		public void CannotParseDuplicateOptions()
		{
			Assert.Throws<DuplicateOptionException>(() => Parse("--duplicate", "--duplicate"));
			Assert.Throws<DuplicateOptionException>(() => Parse("-d", "-d"));
		}

		[Fact]
		public void Parse_Nothing()
		{
			CommandLineArguments args = Parse();
			CheckVerb(args, "");
			CheckArguments(args);
			CheckOptions(args);
		}

		[Fact]
		public void Parse_WithVerb_NoArguments_NoOptions()
		{
			CommandLineArguments args = Parse("verb");
			CheckVerb(args, "verb");
			CheckArguments(args);
			CheckOptions(args);
		}

		[Fact]
		public void Parse_SingleArgument()
		{
			CommandLineArguments args = Parse("verb", "arg");
			CheckVerb(args, "verb");
			CheckArguments(args, "arg");
			CheckOptions(args);
		}

		[Fact]
		public void Parse_MultipleArguments()
		{
			CommandLineArguments args = Parse("verb", "arg1", "arg2", "arg3");
			CheckVerb(args, "verb");
			CheckArguments(args, "arg1", "arg2", "arg3");
			CheckOptions(args);
		}

		[Fact]
		public void Parse_LongSwitch()
		{
			CommandLineArguments args = Parse("verb", "--switch");
			CheckVerb(args, "verb");
			CheckArguments(args);
			CheckOptions(args, "switch", null);
		}

		[Fact]
		public void Parse_ShortSwitch()
		{
			CommandLineArguments args = Parse("verb", "-s");
			CheckVerb(args, "verb");
			CheckArguments(args);
			CheckOptions(args, "s", null);
		}

		[Fact]
		public void Parse_LongOptions()
		{
			CommandLineArguments args = Parse("verb", "--option", "value");
			CheckVerb(args, "verb");
			CheckArguments(args);
			CheckOptions(args, "option", "value");
		}

		[Fact]
		public void Parse_ShortOptions()
		{
			CommandLineArguments args = Parse("verb", "-o", "value");
			CheckVerb(args, "verb");
			CheckArguments(args);
			CheckOptions(args, "o", "value");
		}

		[Fact]
		public void Parse_MixedOptions()
		{
			CommandLineArguments args = Parse("verb", "-s", "-o", "v", "--switch", "--option", "value");
			CheckVerb(args, "verb");
			CheckArguments(args);
			CheckOptions(args, "s", null, "o", "v", "switch", null, "option", "value");
		}

		[Fact]
		public void Parse_All()
		{
			CommandLineArguments args = Parse("verb", "arg", "-s", "-o", "v", "--switch", "--option", "value");
			CheckVerb(args, "verb");
			CheckArguments(args, "arg");
			CheckOptions(args, "s", null, "o", "v", "switch", null, "option", "value");
		}

		private static CommandLineArguments Parse(params string[] args)
		{
			return CommandLineArgumentsParser.Parse(Array.AsReadOnly(args));
		}

		private static void CheckVerb(CommandLineArguments args, string verb)
		{
			Assert.Equal(verb, args.Verb);
		}

		private static void CheckArguments(CommandLineArguments args, params string[] arguments)
		{
			Assert.Equal(arguments, args.Arguments);
		}

		private static void CheckOptions(CommandLineArguments args, params string[] options)
		{
			var expected = new Dictionary<string, string>();

			for (int i = 0; i < options.Length; i += 2)
			{
				string key = options[i];
				string value = options[i + 1];
				expected.Add(key, value);
			}

			Assert.Equal(expected, args.Options);
		}
	}
}
