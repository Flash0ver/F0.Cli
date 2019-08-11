using System;
using System.Collections.Generic;

namespace F0.Cli
{
	internal sealed class CommandLineArguments
	{
		public CommandLineArguments(string verb, IReadOnlyList<string> arguments, IReadOnlyDictionary<string, string> options)
		{
			Verb = verb ?? throw new ArgumentNullException(nameof(verb));
			Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
			Options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public string Verb { get; }
		public IReadOnlyList<string> Arguments { get; }
		public IReadOnlyDictionary<string, string> Options { get; }

		public bool IsDriver => Verb.Length == 0;
		public bool IsCommand => Verb.Length != 0;
		public bool HasArguments => Arguments.Count != 0;
		public bool HasOptions => Options.Count != 0;
	}
}
