using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace F0.Cli
{
	internal static class CommandLineArgumentsParser
	{
		internal static CommandLineArguments Parse(ReadOnlyCollection<string> args)
		{
			_ = args ?? throw new ArgumentNullException(nameof(args));

			return ParseCommandLineArguments(args);
		}

		private static CommandLineArguments ParseCommandLineArguments(ReadOnlyCollection<string> args)
		{
			string? command = null;
			List<string> arguments = new();
			Dictionary<string, string?> options = new(StringComparer.OrdinalIgnoreCase);

			string? previous = null;

			for (int i = 0; i < args.Count; i++)
			{
				string current = args[i];

				if (IsLongSwitch(current))
				{
					previous = GetLongSwitch(current);
					AddOption(previous);
				}
				else if (IsShortSwitch(current))
				{
					previous = GetShortSwitch(current);
					AddOption(previous);
				}
				else if (i == 0)
				{
					Debug.Assert(command is null);
					command = current;
				}
				else if (previous is null)
				{
					arguments.Add(current);
				}
				else
				{
					options[previous] = current;
					previous = null;
				}
			}

			return new CommandLineArguments(command ?? String.Empty, arguments, options);

			void AddOption(string option)
			{
				if (option.Length == 0)
				{
					throw new AnonymousOptionException();
				}

				if (options.ContainsKey(option))
				{
					throw new DuplicateOptionException(option);
				}
				else
				{
					options.Add(option, null);
				}
			}
		}

		private static bool IsShortSwitch(string arg)
		{
			return arg.StartsWith("-", StringComparison.OrdinalIgnoreCase)
				&& !IsNegativeNumber(arg);

			static bool IsNegativeNumber(string arg)
			{
				const char decimalPoint = '.';

				return (arg.Length > 2 && arg[1] == decimalPoint && Char.IsDigit(arg[2]))
					|| (arg.Length > 1 && Char.IsDigit(arg[1]));
			}
		}

		private static bool IsLongSwitch(string arg)
		{
			return arg.StartsWith("--", StringComparison.OrdinalIgnoreCase);
		}

		private static string GetShortSwitch(string arg)
		{
			return arg.Substring(1);
		}

		private static string GetLongSwitch(string arg)
		{
			return arg.Substring(2);
		}
	}
}
