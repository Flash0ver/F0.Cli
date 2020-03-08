using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;
using F0.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace F0.Tests.Shared
{
	internal sealed class CommandLineBackgroundServiceUnit
	{
		private readonly CommandContext context;
		private readonly IHostedService hostedService;
		private readonly TaskCompletionSource<bool> commandPipelineOperation;

		public CommandLineBackgroundServiceUnit(params string[] args)
			: this(null, args)
		{
		}

		public CommandLineBackgroundServiceUnit(string verb, Action<IServiceCollection> configureDelegate)
			: this(configureDelegate, new string[] { verb })
		{
		}

		private CommandLineBackgroundServiceUnit(Action<IServiceCollection> configureDelegate, string[] args)
		{
			Reporter = new TestReporter();

			IServiceCollection collection = new ServiceCollection();
			configureDelegate?.Invoke(collection);

			IServiceProviderFactory<IServiceCollection> factory = new DefaultServiceProviderFactory();
			IServiceProvider provider = factory.CreateServiceProvider(collection);

			IHostApplicationLifetime appLifetime = new TestApplicationLifetime(() =>
			{
				StopCount++;
				commandPipelineOperation.SetResult(true);
			});

			Assembly assembly = typeof(CommandLineBackgroundServiceUnit).Assembly;

			context = new CommandContext(args, assembly);
			hostedService = new CommandLineBackgroundService(provider, appLifetime, context, Reporter);
			commandPipelineOperation = new TaskCompletionSource<bool>();
		}

		internal TestReporter Reporter { get; }
		internal int StopCount { get; private set; }

		internal async Task RunAsync(CancellationToken cancellationToken = default)
		{
			await ExecuteAsync(hostedService, cancellationToken);

			Task<bool> task = commandPipelineOperation.Task;
			TimeSpan timeout = TimeSpan.FromMilliseconds(100);
			if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) != task)
			{
				throw new TimeoutException();
			}
		}

		internal CommandResult GetResult()
		{
			return context.GetResult();
		}

		internal void CheckCompletion()
		{
			Reporter.CheckEmpty();
		}

		private static Task ExecuteAsync(IHostedService hostedService, CancellationToken cancellationToken)
		{
			//Microsoft.Extensions.Hosting.BackgroundService.ExecuteAsync

			Type type = hostedService.GetType();
			MethodInfo mi = type.GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance);

			object value = mi.Invoke(hostedService, new object[] { cancellationToken });
			var task = value as Task;
			return task;
		}
	}
}
