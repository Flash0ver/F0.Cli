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
		private readonly IHostEnvironment environment;
		private readonly IReporter reporter;
		private readonly INuGetClient nuGetService;

		public PackageCommand(IHostEnvironment env, IReporter reporter, INuGetClient nuGetService)
		{
			reporter.WriteInfo($"Creating {nameof(PackageCommand)}");

			environment = env;
			this.reporter = reporter;
			this.nuGetService = nuGetService;
		}

		public IEnumerable<string> Arguments { get; set; }

		public string Owner { get; set; }
		public string Tag { get; set; }
		public int Skip { get; set; }
		public int Take { get; set; }

		public override async Task<CommandResult> ExecuteAsync(CancellationToken cancellationToken)
		{
			Arguments ??= Enumerable.Empty<string>();

			reporter.WriteInfo($"Executing {nameof(PackageCommand)}");
			reporter.WriteInfo($"Arguments: {String.Join(", ", Arguments)}");
			reporter.WriteInfo($"Option {nameof(Owner)}: {Owner ?? "<null>"}");
			reporter.WriteInfo($"Option {nameof(Tag)}: {Tag ?? "<null>"}");
			reporter.WriteInfo($"Option {nameof(Skip)}: {Skip}");
			reporter.WriteInfo($"Option {nameof(Take)}: {Take}");
			reporter.WriteLine();

			reporter.WriteInfo($"Hosting environment: {environment.EnvironmentName}");
			reporter.WriteInfo($"Application name: {environment.ApplicationName}");
			reporter.WriteInfo($"Content root path: {environment.ContentRootPath}");
			reporter.WriteLine();

			if (Owner is { })
			{
				string info = await nuGetService.GetByOwnerAsync(Owner, cancellationToken);
				reporter.WriteInfo(info);
			}

			reporter.WriteLine();

			foreach (string argument in Arguments)
			{
				string package = await nuGetService.GetByIdAsync(argument, cancellationToken);
				reporter.WriteInfo($"- {package}");
			}

			reporter.WriteLine();

			if (Tag is { })
			{
				string data = await nuGetService.GetByTagAsync(Tag, Skip, Take, cancellationToken);
				reporter.WriteInfo(data);
			}

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
