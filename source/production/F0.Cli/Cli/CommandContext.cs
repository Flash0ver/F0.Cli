using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace F0.Cli
{
	internal sealed class CommandContext
	{
		private CommandResult result;

		internal CommandContext(string[] args, Assembly commandAssembly)
		{
			CommandLineArgs = args is null
				? throw new ArgumentNullException(nameof(args))
				: Array.AsReadOnly(args);
			CommandAssembly = commandAssembly ?? throw new ArgumentNullException(nameof(commandAssembly));
		}

		internal ReadOnlyCollection<string> CommandLineArgs { get; }
		internal Assembly CommandAssembly { get; }

		internal CommandResult GetResult()
		{
			return result ?? throw new InvalidOperationException("Result not set.");
		}

		internal void SetResult(CommandResult result)
		{
			if (this.result is { })
			{
				throw new InvalidOperationException("Result already set.");
			}

			this.result = result ?? throw new ArgumentNullException(nameof(result));
		}
	}
}
