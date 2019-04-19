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
			if (args is null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			CommandLineArgs = Array.AsReadOnly(args);
			CommandAssembly = commandAssembly ?? throw new ArgumentNullException(nameof(commandAssembly));
		}

		internal ReadOnlyCollection<string> CommandLineArgs { get; }
		internal Assembly CommandAssembly { get; }

		internal CommandResult GetResult()
		{
			if (result is null)
			{
				throw new InvalidOperationException("Result not set.");
			}

			return result;
		}

		internal void SetResult(CommandResult result)
		{
			if (!(this.result is null))
			{
				throw new InvalidOperationException("Result already set.");
			}

			this.result = result ?? throw new ArgumentNullException(nameof(result));
		}
	}
}
