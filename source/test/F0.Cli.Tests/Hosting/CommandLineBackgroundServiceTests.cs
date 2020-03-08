using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;
using F0.Hosting;
using F0.Logging;
using F0.Tests.Commands;
using F0.Tests.Shared;
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

			unit.Reporter.CheckNextError($"A suitable constructor for type '{typeof(InvalidCommand)}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_InvalidArguments()
		{
			var unit = new CommandLineBackgroundServiceUnit("null", "args");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError($"No bindable Arguments property defined by the Command type '{typeof(NullCommand)}'.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_InvalidOptions()
		{
			var unit = new CommandLineBackgroundServiceUnit("null", "--option", "value");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError($"Bindable Option 'option' not defined by the Command type '{typeof(NullCommand)}'.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Cancellation_CanceledCommand_Synchronous()
		{
			var unit = new CommandLineBackgroundServiceUnit("cancellation");

			await unit.RunAsync(new CancellationToken(true));

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandExecutionCanceled, result.ExitCode);

			unit.Reporter.CheckNextWarning($"The command '{nameof(CancellationCommand)}' was canceled.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Cancellation_CanceledCommand_Asynchronous()
		{
			var unit = new CommandLineBackgroundServiceUnit("cancel");

			await unit.RunAsync(new CancellationToken(true));

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandExecutionCanceled, result.ExitCode);

			unit.Reporter.CheckNextWarning($"The command '{nameof(CancelCommand)}' was canceled.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Cancellation_GracefulTermination_InResponseToCancellationRequest()
		{
			var unit = new CommandLineBackgroundServiceUnit("longrunning");

			using var cts = new CancellationTokenSource();
			Task task = unit.RunAsync(cts.Token);

			unit.Reporter.CheckEmpty();

			cts.Cancel();
			await task;

			CommandResult result = unit.GetResult();
			Assert.Equal(0, result.ExitCode);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_CommandThrowsException()
		{
			var unit = new CommandLineBackgroundServiceUnit("exception");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandExecutionFaulted, result.ExitCode);

			unit.Reporter.CheckNextError("An exceptional situation has occurred.");
			unit.CheckCompletion();
		}
	}
}
