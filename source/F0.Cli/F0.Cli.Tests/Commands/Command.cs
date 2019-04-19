using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class Command : CommandBase
	{
		public Command()
		{
		}

		public IEnumerable<string> Arguments { get; set; }

		public string Option { get; set; }

		public override Task<CommandResult> ExecuteAsync()
		{
			throw new NotImplementedException();
		}
	}
}
