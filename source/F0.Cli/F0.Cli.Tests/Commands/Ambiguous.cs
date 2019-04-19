﻿using System;
using System.Threading.Tasks;
using F0.Cli;

namespace F0.Tests.Commands
{
	public sealed class Ambiguous : CommandBase
	{
		public Ambiguous()
		{
		}

		public override Task<CommandResult> ExecuteAsync()
		{
			throw new NotSupportedException();
		}
	}
}
