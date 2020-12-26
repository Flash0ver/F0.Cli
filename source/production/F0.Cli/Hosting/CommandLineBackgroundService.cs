using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using F0.Cli;
using F0.IO;
using F0.Logging;
using F0.Reflection;
using Microsoft.Extensions.Hosting;

namespace F0.Hosting
{
	internal sealed class CommandLineBackgroundService : BackgroundService
	{
		private readonly IServiceProvider provider;
		private readonly IHostApplicationLifetime appLifetime;
		private readonly CommandContext context;
		private readonly IReporter reporter;

		public CommandLineBackgroundService(IServiceProvider provider, IHostApplicationLifetime appLifetime, CommandContext context, IReporter reporter)
		{
			this.provider = provider;
			this.appLifetime = appLifetime;
			this.context = context;
			this.reporter = reporter;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			CommandResult result = await ExecuteCommandPipelineAsync(context.CommandLineArgs, context.CommandAssembly, provider, reporter, stoppingToken);
			context.SetResult(result);

			appLifetime.StopApplication();
		}

		private static async Task<CommandResult> ExecuteCommandPipelineAsync(ReadOnlyCollection<string> commandLineArguments, Assembly commandAssembly, IServiceProvider provider, IReporter reporter, CancellationToken stoppingToken)
		{
			CommandResult result;

			try
			{
				result = await RunCommandPipelineAsync(commandLineArguments, commandAssembly, provider, stoppingToken);
			}
			catch (CommandCanceledException exception)
			{
				reporter.WriteWarning(exception.Message);
				result = new CommandResult(LoggingEvents.CommandExecutionCanceled);
			}
			catch (CommandExecutionException exception)
			{
				reporter.WriteError(exception.InnerException.Message);
				result = new CommandResult(LoggingEvents.CommandExecutionFaulted);
			}
			catch (Exception exception)
			{
				reporter.WriteError(exception.Message);
				result = new CommandResult(LoggingEvents.CommandPipelineFailure);
			}

			return result;
		}

		private static async Task<CommandResult> RunCommandPipelineAsync(ReadOnlyCollection<string> commandLineArguments, Assembly commandAssembly, IServiceProvider provider, CancellationToken stoppingToken)
		{
			CommandLineArguments args = CommandLineArgumentsParser.Parse(commandLineArguments);
			Type type = CommandSelector.SelectCommand(commandAssembly, args);

			using CommandBase instance = CommandActivator.ConstructCommand(provider, type);
			CommandArgumentsBinder.BindArguments(instance, args);
			CommandOptionsBinder.BindOptions(instance, args);

			CommandResult result = await CommandExecutor.InvokeAsync(instance, stoppingToken);
			return result;
		}
	}
}
