using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli.Example.Http;
using F0.IO;
using Microsoft.Extensions.Hosting;

namespace F0.Cli.Example.Commands
{
	public sealed class PackageCommand : CommandBase
	{
		private readonly IHostingEnvironment environment;
		private readonly IReporter reporter;
		private readonly INuGetClient nuGetService;

		public PackageCommand(IHostingEnvironment env, IReporter reporter, INuGetClient nuGetService)
		{
			reporter.WriteInfo($"Creating {nameof(PackageCommand)}");

			environment = env;
			this.reporter = reporter;
			this.nuGetService = nuGetService;
		}

		public IEnumerable<string> Arguments { get; set; }

		public string Author { get; set; }

		public override async Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			if (Arguments is null)
			{
				Arguments = Enumerable.Empty<string>();
			}

			reporter.WriteInfo($"Executing {nameof(PackageCommand)}");
			reporter.WriteInfo($"Arguments: {String.Join(", ", Arguments)}");
			reporter.WriteInfo($"Option {nameof(Author)}: {Author ?? "<null>"}");
			reporter.WriteLine();

			reporter.WriteInfo($"Hosting environment: {environment.EnvironmentName}");
			reporter.WriteInfo($"Application name: {environment.ApplicationName}");
			reporter.WriteInfo($"Content root path: {environment.ContentRootPath}");
			reporter.WriteLine();

			if (!(Author is null))
			{
				string info = await nuGetService.GetByAuthorAsync(Author, cancellationToken);
				reporter.WriteInfo(info);
			}

			reporter.WriteLine();

			foreach (string argument in Arguments)
			{
				string package = await nuGetService.GetByIdAsync(argument, cancellationToken);
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
