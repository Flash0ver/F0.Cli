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

		[Fact]
		public void Parse_NoVerb_VerbMustBeFirst()
		{
			CommandLineArguments args = Parse("--option", "value", "arg");
			CheckVerb(args, "");
			CheckArguments(args, "arg");
			CheckOptions(args, "option", "value");
		}

		[Fact]
		public void Parse_ArgumentsMayBeSeparated_WithVerb()
		{
			CommandLineArguments args = Parse("verb", "arg1", "-o", "v", "arg2", "--option", "value", "arg3");
			CheckVerb(args, "verb");
			CheckArguments(args, "arg1", "arg2", "arg3");
			CheckOptions(args, "o", "v", "option", "value");
		}

		[Fact]
		public void Parse_ArgumentsMayBeSeparated_WithoutVerb()
		{
			CommandLineArguments args = Parse("-o", "v", "arg1", "arg2", "--option", "value", "arg3");
			CheckVerb(args, "");
			CheckArguments(args, "arg1", "arg2", "arg3");
			CheckOptions(args, "o", "v", "option", "value");
		}

		[Fact]
		public void Parse_NumericValue()
		{
			CommandLineArguments args = Parse("verb", "--integral", "240", "--floating-point", "240.042");
			CheckVerb(args, "verb");
			CheckArguments(args);
			CheckOptions(args, "integral", "240", "floating-point", "240.042");
		}

		[Fact]
		public void Parse_SingleDashFollowedByDigit_MayBeSignedNumber()
		{
			CommandLineArguments args = Parse("-o", "-240");
			CheckVerb(args, "");
			CheckArguments(args);
			CheckOptions(args, "o", "-240");
		}

		[Fact]
		public void Parse_DoubleDashFollowedByDigit_MayNotBeSignedNumber()
		{
			CommandLineArguments args = Parse("--option", "--240");
			CheckVerb(args, "");
			CheckArguments(args);
			CheckOptions(args, "option", null, "240", null);
		}

		[Theory]
		[MemberData(nameof(GetIntegerLiteralTestData))]
		[MemberData(nameof(GetRealLiteralTestData))]
		public void Parse_NumericLiteral(string value)
		{
			CommandLineArguments args = Parse("--number", value);
			CheckVerb(args, "");
			CheckArguments(args);
			CheckOptions(args, "number", value);
		}

		[Theory]
		[InlineData("win-x86")]
		[InlineData("linux-arm")]
		[InlineData("osx-x64")]
		public void Parse_DashOccursWithinValue_OptionHasValue(string value)
		{
			CommandLineArguments args = Parse("--runtime", value);
			CheckVerb(args, "");
			CheckArguments(args);
			CheckOptions(args, "runtime", value);
		}

		[Theory]
		[InlineData("1.0.0-alpha+001")]
		[InlineData("1.0.0+20130313144700")]
		[InlineData("1.0.0-beta+exp.sha.5114f85")]
		[InlineData("1.0.0+21AF26D3—-117B344092BD")]
		public void Parse_PlusOccursWithinValue_OptionHasValue(string value)
		{
			CommandLineArguments args = Parse("--semver", value);
			CheckVerb(args, "");
			CheckArguments(args);
			CheckOptions(args, "semver", value);
		}

		[Theory]
		[InlineData("README.md")]
		[InlineData("LICENSE")]
		[InlineData(".gitignore")]
		[InlineData("src")]
		[InlineData("../")]
		[InlineData("./")]
		[InlineData("src/Directory.Build.props")]
		[InlineData("src\\Directory.Build.props")]
		[InlineData(".vscode/extensions.json")]
		[InlineData(".vscode\\extensions.json")]
		[InlineData(".vs/.suo")]
		[InlineData(".vs\\.suo")]
		public void Parse_Path_OptionHasValue(string value)
		{
			CommandLineArguments args = Parse("--option", value);
			CheckVerb(args, "");
			CheckArguments(args);
			CheckOptions(args, "option", value);
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

		public static TheoryData<string> GetIntegerLiteralTestData()
		{
			return new TheoryData<string>()
			{
				// no suffix
				{ "0" },
				{ "+0" },
				{ "-0" },
				// uint, ulong
				{ "1U" },
				{ "1u" },
				// long, ulong
				{ "1L" },
				{ "-1L" },
				{ "1l" },
				{ "-1l" },
				// ulong
				{ "1UL" },
				{ "1Ul" },
				{ "1uL" },
				{ "1ul" },
				{ "1LU" },
				{ "1Lu" },
				{ "1lU" },
				{ "1lu" },
			};
		}

		public static TheoryData<string> GetRealLiteralTestData()
		{
			return new TheoryData<string>
			{
				// without suffix
				{ "0.0" },
				{ "+0.0" },
				{ "-0.0" },
				{ ".0" },
				{ "+.0" },
				{ "-.0" },
				// float
				{ "1f" },
				{ "-1f" },
				{ "1F" },
				{ "-1F" },
				// double
				{ "1d" },
				{ "-1d" },
				{ "1D" },
				{ "-1D" },
				// decimal
				{ "1m" },
				{ "-1m" },
				{ "1M" },
				{ "-1M" },
			};
		}
	}
}
