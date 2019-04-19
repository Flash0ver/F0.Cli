using System;
using System.Threading.Tasks;

namespace F0.Cli
{
	public abstract class CommandBase : IDisposable
	{
		protected CommandBase()
		{
		}

		public abstract Task<CommandResult> ExecuteAsync();

		protected CommandResult Success()
		{
			return new CommandResult(0);
		}

		protected CommandResult Error()
		{
			return new CommandResult(1);
		}

		public virtual void Dispose()
		{
		}
	}
}
