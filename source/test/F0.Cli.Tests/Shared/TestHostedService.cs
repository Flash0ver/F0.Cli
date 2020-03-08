using System.Threading;
using System.Threading.Tasks;
using F0.Cli;
using Microsoft.Extensions.Hosting;

namespace F0.Tests.Shared
{
	internal sealed class TestHostedService : IHostedService
	{
		private readonly IHostApplicationLifetime appLifetime;
		private readonly CommandContext context;

		public TestHostedService(IHostApplicationLifetime appLifetime, CommandContext context)
		{
			this.appLifetime = appLifetime;
			this.context = context;
		}

		Task IHostedService.StartAsync(CancellationToken cancellationToken)
		{
			appLifetime.StopApplication();
			return Task.CompletedTask;
		}

		Task IHostedService.StopAsync(CancellationToken cancellationToken)
		{
			context.SetResult(new CommandResult(240));
			return Task.CompletedTask;
		}
	}
}
