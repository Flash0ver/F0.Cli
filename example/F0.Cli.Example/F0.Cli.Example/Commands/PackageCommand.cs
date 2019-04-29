using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using F0.Cli.Example.Http;
using F0.IO;

namespace F0.Cli.Example.Commands
{
	public sealed class PackageCommand : CommandBase
	{
		private readonly IReporter reporter;
		private readonly INuGetClient nuGetService;

		public PackageCommand(IReporter reporter, INuGetClient nuGetService)
		{
			reporter.WriteInfo($"Creating {nameof(PackageCommand)}");

			this.reporter = reporter;
			this.nuGetService = nuGetService;
		}

		public IEnumerable<string> Arguments { get; set; }

		public string Author { get; set; }

		public override async Task<CommandResult> ExecuteAsync()
		{
			reporter.WriteInfo($"Executing {nameof(PackageCommand)}");
			reporter.WriteInfo($"Arguments: {String.Join(", ", Arguments)}");
			reporter.WriteInfo($"Option {nameof(Author)}: {Author ?? "<null>"}");
			reporter.WriteLine();

			if (!(Author is null))
			{
				string info = await nuGetService.GetByAuthorAsync(Author);
				reporter.WriteInfo(info);
			}

			reporter.WriteLine();

			foreach (string argument in Arguments)
			{
				string package = await nuGetService.GetByIdAsync(argument);
				reporter.WriteInfo($"- {package}");
			}

			reporter.WriteLine();
			reporter.WriteInfo($"Executed {nameof(PackageCommand)}");
			return Success();
		}

		public override void Dispose()
		{
			reporter.WriteInfo($"Disposing {nameof(PackageCommand)}");

			base.Dispose();
		}
	}
}
