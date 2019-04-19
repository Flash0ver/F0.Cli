using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;
using F0.Hosting;
using F0.Logging;
using F0.Tests.Commands;
using F0.Tests.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace F0.Tests.Hosting
{
	public class CommandLineBackgroundServiceTests
	{
		[Fact]
		public void CanBeRegisteredAndCanBeResolved()
		{
			ConstructorInfo constructor = typeof(CommandLineBackgroundService).GetConstructors().Single();
			Assert.True(constructor.IsPublic);
		}

		[Fact]
		public void IsLongRunning()
		{
			Type type = typeof(CommandLineBackgroundService);
			Assert.Equal(typeof(BackgroundService), type.BaseType);
		}

		[Fact]
		public async Task ResultIsAvailableAfterCommandPipelineExecution()
		{
			var unit = new CommandLineBackgroundServiceUnit("null");

			Assert.Throws<InvalidOperationException>(() => unit.GetResult());
			await unit.RunAsync();
			Assert.Equal(0, unit.GetResult().ExitCode);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task ApplicationIsStoppedAfterCommandPipelineExecution()
		{
			var unit = new CommandLineBackgroundServiceUnit("null");

			Assert.Equal(0, unit.StopCount);
			await unit.RunAsync();
			Assert.Equal(1, unit.StopCount);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task CommandExecutionMayCompleteAsynchronously()
		{
			var unit = new CommandLineBackgroundServiceUnit("async");

			Assert.Throws<InvalidOperationException>(() => unit.GetResult());
			Assert.Equal(0, unit.StopCount);

			await unit.RunAsync();

			Assert.Equal(0, unit.GetResult().ExitCode);
			Assert.Equal(1, unit.StopCount);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task CommandIsDisposedAfterCommandPipelineExecution()
		{
			var tracer = new CommandDependency();
			var unit = new CommandLineBackgroundServiceUnit("dependency", services => services.AddSingleton(_ => tracer));

			Assert.Empty(tracer.CallLog);
			await unit.RunAsync();
			Assert.Equal(new string[] { ".ctor", nameof(CommandBase.ExecuteAsync), nameof(IDisposable.Dispose) }, tracer.CallLog);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Successfully()
		{
			var unit = new CommandLineBackgroundServiceUnit("exitcode");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(0, result.ExitCode);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Successfully_CommandLineArguments()
		{
			var unit = new CommandLineBackgroundServiceUnit("exitcode", "--exitcode", "240");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(240, result.ExitCode);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_InvalidCommandLineArguments()
		{
			var unit = new CommandLineBackgroundServiceUnit(null, "");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError(new NullReferenceException().Message);
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_CommandNotFound()
		{
			var unit = new CommandLineBackgroundServiceUnit("_");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError("Command '_' not found.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_CommandCannotBeActivated()
		{
			var unit = new CommandLineBackgroundServiceUnit("invalid");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError("A suitable constructor for type 'F0.Tests.Commands.InvalidCommand' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_InvalidArguments()
		{
			var unit = new CommandLineBackgroundServiceUnit("null", "args");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError("No bindable Arguments property defined by the Command type 'F0.Tests.Commands.NullCommand'.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_InvalidOptions()
		{
			var unit = new CommandLineBackgroundServiceUnit("null", "--option", "value");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError("Bindable Option 'option' not defined by the Command type 'F0.Tests.Commands.NullCommand'.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_CommandCannotBeExecuted()
		{
			var unit = new CommandLineBackgroundServiceUnit("error");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandExecutionFailed, result.ExitCode);

			unit.Reporter.CheckNextError("Operation is not valid due to the current state of the object.");
			unit.CheckCompletion();
		}
	}

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

			IApplicationLifetime appLifetime = new TestApplicationLifetime(() =>
			{
				StopCount++;
				commandPipelineOperation.SetResult(true);
			});

			Assembly assembly = typeof(CommandLineBackgroundServiceTests).Assembly;

			context = new CommandContext(args, assembly);
			hostedService = new CommandLineBackgroundService(provider, appLifetime, context, Reporter);
			commandPipelineOperation = new TaskCompletionSource<bool>();
		}

		internal TestReporter Reporter { get; }
		internal int StopCount { get; private set; }

		internal async Task RunAsync()
		{
			await hostedService.StartAsync(CancellationToken.None);

			Task<bool> task = commandPipelineOperation.Task;
			TimeSpan timeout = new HostOptions().ShutdownTimeout;
			if (await Task.WhenAny(task, Task.Delay(timeout)) != task)
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
	}

	internal sealed class TestApplicationLifetime : IApplicationLifetime
	{
		private readonly Action onStopping;

		public TestApplicationLifetime(Action onStopping)
		{
			this.onStopping = onStopping;
		}

		CancellationToken IApplicationLifetime.ApplicationStarted { get; }
		CancellationToken IApplicationLifetime.ApplicationStopping { get; }
		CancellationToken IApplicationLifetime.ApplicationStopped { get; }

		void IApplicationLifetime.StopApplication()
		{
			onStopping();
		}
	}
}
