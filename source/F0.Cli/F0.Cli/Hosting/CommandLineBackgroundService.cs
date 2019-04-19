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
		private readonly IApplicationLifetime appLifetime;
		private readonly CommandContext context;
		private readonly IReporter reporter;

		public CommandLineBackgroundService(IServiceProvider provider, IApplicationLifetime appLifetime, CommandContext context, IReporter reporter)
		{
			this.provider = provider;
			this.appLifetime = appLifetime;
			this.context = context;
			this.reporter = reporter;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			CommandResult result = await ExecuteCommandPipelineAsync(context.CommandLineArgs, context.CommandAssembly, provider, reporter);
			context.SetResult(result);

			appLifetime.StopApplication();
		}

		private static async Task<CommandResult> ExecuteCommandPipelineAsync(ReadOnlyCollection<string> commandLineArguments, Assembly commandAssembly, IServiceProvider provider, IReporter reporter)
		{
			CommandResult result;

			try
			{
				result = await RunCommandPipelineAsync(commandLineArguments, commandAssembly, provider);
			}
			catch (CommandExecutionException exception)
			{
				reporter.WriteError(exception.InnerException.Message);
				result = new CommandResult(LoggingEvents.CommandExecutionFailed);
			}
			catch (Exception exception)
			{
				reporter.WriteError(exception.Message);
				result = new CommandResult(LoggingEvents.CommandPipelineFailure);
			}

			return result;
		}

		private static async Task<CommandResult> RunCommandPipelineAsync(ReadOnlyCollection<string> commandLineArguments, Assembly commandAssembly, IServiceProvider provider)
		{
			CommandLineArguments args = CommandLineArgumentsParser.Parse(commandLineArguments);

			Type type = CommandSelector.SelectCommand(commandAssembly, args);
			using (CommandBase instance = CommandActivator.ConstructCommand(provider, type))
			{
				CommandArgumentsBinder.BindArguments(instance, args);
				CommandOptionsBinder.BindOptions(instance, args);

				CommandResult result = await CommandExecutor.InvokeAsync(instance);
				return result;
			}
		}
	}
}
