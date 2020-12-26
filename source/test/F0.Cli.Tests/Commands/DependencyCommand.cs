using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class DependencyCommand : CommandBase
	{
		private readonly CommandDependency dependency;

		public DependencyCommand(CommandDependency dependency)
		{
			this.dependency = dependency;

			dependency.TraceCall();
		}

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			dependency.TraceCall();

			return Task.FromResult(Success());
		}

		public override void Dispose()
		{
			base.Dispose();

			dependency.TraceCall();
		}
	}

	public sealed class CommandDependency
	{
		private readonly List<string> callLog;

		public CommandDependency()
		{
			callLog = new List<string>();
		}

		public IEnumerable<string> CallLog => callLog;

		internal void TraceCall([CallerMemberName] string memberName = "")
		{
			callLog.Add(memberName);
		}
	}
}
