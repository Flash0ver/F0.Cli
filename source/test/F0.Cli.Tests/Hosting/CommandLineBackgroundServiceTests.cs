using System;
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
			ConstructorInfo[] constructors = typeof(CommandLineBackgroundService).GetConstructors();
			ConstructorInfo constructor = Assert.Single(constructors);
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
			CommandLineBackgroundServiceUnit unit = new(NullCommand.Name);

			Assert.Throws<InvalidOperationException>(() => unit.GetResult());
			await unit.RunAsync();
			Assert.Equal(0, unit.GetResult().ExitCode);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task ApplicationIsStoppedAfterCommandPipelineExecution()
		{
			CommandLineBackgroundServiceUnit unit = new(NullCommand.Name);

			Assert.Equal(0, unit.StopCount);
			await unit.RunAsync();
			Assert.Equal(1, unit.StopCount);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task CommandExecutionMayCompleteAsynchronously()
		{
			CommandLineBackgroundServiceUnit unit = new(AsyncCommand.Name);

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
			CommandDependency tracer = new();
			CommandLineBackgroundServiceUnit unit = new(DependencyCommand.Name, services => services.AddSingleton(_ => tracer));

			Assert.Empty(tracer.CallLog);
			await unit.RunAsync();
			Assert.Equal(new string[] { ".ctor", nameof(CommandBase.ExecuteAsync), nameof(IDisposable.Dispose) }, tracer.CallLog);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Successfully()
		{
			CommandLineBackgroundServiceUnit unit = new(ExitCodeCommand.Name);

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(0, result.ExitCode);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Successfully_CommandLineArguments()
		{
			CommandLineBackgroundServiceUnit unit = new(ExitCodeCommand.Name, "--exitcode", "240");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(240, result.ExitCode);

			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_InvalidCommandLineArguments()
		{
			CommandLineBackgroundServiceUnit unit = new(null!, "");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError(new NullReferenceException().Message);
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_CommandNotFound()
		{
			CommandLineBackgroundServiceUnit unit = new("_");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError("Command '_' not found.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_CommandCannotBeActivated()
		{
			CommandLineBackgroundServiceUnit unit = new(InvalidCommand.Name);

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError($"A suitable constructor for type '{typeof(InvalidCommand)}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_InvalidArguments()
		{
			CommandLineBackgroundServiceUnit unit = new(NullCommand.Name, "args");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError($"No bindable Arguments property defined by the Command type '{typeof(NullCommand)}'.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_InvalidOptions()
		{
			CommandLineBackgroundServiceUnit unit = new(NullCommand.Name, "--option", "value");

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandPipelineFailure, result.ExitCode);

			unit.Reporter.CheckNextError($"Bindable Option 'option' not defined by the Command type '{typeof(NullCommand)}'.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Cancellation_CanceledCommand_Synchronous()
		{
			CommandLineBackgroundServiceUnit unit = new(CancellationCommand.Name);

			await unit.RunAsync(new CancellationToken(true));

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandExecutionCanceled, result.ExitCode);

			unit.Reporter.CheckNextWarning($"The command '{nameof(CancellationCommand)}' was canceled.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Cancellation_CanceledCommand_Asynchronous()
		{
			CommandLineBackgroundServiceUnit unit = new(CancelCommand.Name);

			await unit.RunAsync(new CancellationToken(true));

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandExecutionCanceled, result.ExitCode);

			unit.Reporter.CheckNextWarning($"The command '{nameof(CancelCommand)}' was canceled.");
			unit.CheckCompletion();
		}

		[Fact]
		public async Task RunCommandPipeline_Cancellation_GracefulTermination_InResponseToCancellationRequest()
		{
			CommandLineBackgroundServiceUnit unit = new(LongRunningCommand.Name);

			using CancellationTokenSource cts = new();
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
			CommandLineBackgroundServiceUnit unit = new(ExceptionCommand.Name);

			await unit.RunAsync();

			CommandResult result = unit.GetResult();
			Assert.Equal(LoggingEvents.CommandExecutionFaulted, result.ExitCode);

			unit.Reporter.CheckNextError("An exceptional situation has occurred.");
			unit.CheckCompletion();
		}
	}
}
