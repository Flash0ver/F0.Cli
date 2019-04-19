using System;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	internal sealed class InternalCommand : CommandBase
	{
		internal static CommandBase CreateSuccess(Action onDispose)
		{
			return new InternalCommand(onDispose)
			{
				isError = false
			};
		}

		internal static CommandBase CreateError(Action onDispose)
		{
			return new InternalCommand(onDispose)
			{
				isError = true
			};
		}

		private bool isError;
		private readonly Action onDispose;

		private InternalCommand(Action onDispose)
		{
			this.onDispose = onDispose;
		}

		public override Task<CommandResult> ExecuteAsync()
		{
			CommandResult result = isError ? Error() : Success();

			return Task.FromResult(result);
		}

		public override void Dispose()
		{
			onDispose();
		}
	}
}
