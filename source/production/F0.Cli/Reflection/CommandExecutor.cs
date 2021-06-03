using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Reflection
{
	internal static class CommandExecutor
	{
		internal static async Task<CommandResult> InvokeAsync(CommandBase command, CancellationToken cancellationToken)
		{
			_ = command ?? throw new ArgumentNullException(nameof(command));

			CommandResult result;

			try
			{
				result = await command.ExecuteAsync(cancellationToken);
			}
			catch (OperationCanceledException exception)
			{
				throw new CommandCanceledException(command, exception);
			}
			catch (Exception exception)
			{
				throw new CommandExecutionException(command, exception);
			}

			return result;
		}
	}
}
