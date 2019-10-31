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
		public void ExecuteCommand_ReturnExitCodeFromCommandResult_Synchronous()
		{
			IHost host = BuildHost();

			int exitCode = host.RunCli();

			Assert.Equal(240, exitCode);
			CheckDisposed(host);
		}

		[Fact]
		public async Task ExecuteCommand_ReturnExitCodeFromCommandResult_Asynchronous()
		{
			IHost host = BuildHost();

			int exitCode = await host.RunCliAsync();

			Assert.Equal(240, exitCode);
			CheckDisposed(host);
		}

		[Fact]
		public async Task ShutdownApplication()
		{
			IHost host = BuildHost();

			Task<int> cliTask = host.RunCliAsync(new CancellationToken(true));

			await Assert.ThrowsAsync<OperationCanceledException>(() => cliTask);
			Assert.True(cliTask.IsCanceled);
			CheckDisposed(host);
		}

		private static IHost BuildHost()
		{
			Assembly assembly = typeof(HostExtensionsTests).Assembly;

			IHostBuilder hostBuilder = new HostBuilder();
			hostBuilder.ConfigureServices((ctx, services) =>
			{
				services.AddSingleton(sp => new CommandContext(Array.Empty<string>(), assembly));
				services.AddHostedService<TestHostedService>();
			});

			IHost host = hostBuilder.Build();
			return host;
		}

		private static void CheckDisposed(IHost host)
		{
			Assert.Throws<ObjectDisposedException>(() => host.Services.GetService(typeof(object)));
		}
	}

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
