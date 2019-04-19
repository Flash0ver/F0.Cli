using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;
using F0.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace F0.Tests.Hosting
{
	public class HostExtensionsTests
	{
		[Fact]
		public void ReturnExitCodeFromResult()
		{
			Assembly assembly = typeof(HostExtensionsTests).Assembly;

			IHostBuilder hostBuilder = new HostBuilder();
			hostBuilder.ConfigureServices((ctx, services) =>
			{
				services.AddSingleton(sp => new CommandContext(Array.Empty<string>(), assembly));
				services.AddHostedService<TestHostedService>();
			});
			IHost host = hostBuilder.Build();

			int exitCode = host.RunCli();

			Assert.Equal(240, exitCode);
		}
	}

	internal sealed class TestHostedService : IHostedService
	{
		private readonly IApplicationLifetime appLifetime;
		private readonly CommandContext context;

		public TestHostedService(IApplicationLifetime appLifetime, CommandContext context)
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
