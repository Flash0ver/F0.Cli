using System;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Reflection
{
	internal static class CommandExecutor
	{
		internal static async Task<CommandResult> InvokeAsync(CommandBase command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			CommandResult result;

			try
			{
				result = await command.ExecuteAsync();
			}
			catch (Exception exception)
			{
				throw new CommandExecutionException(command, exception);
			}

			return result;
		}
	}
}
