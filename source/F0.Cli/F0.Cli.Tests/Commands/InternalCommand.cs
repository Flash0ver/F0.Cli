﻿using System;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	internal sealed class InternalCommand : CommandBase
	{
		public InternalCommand()
		{
		}

		public override Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
