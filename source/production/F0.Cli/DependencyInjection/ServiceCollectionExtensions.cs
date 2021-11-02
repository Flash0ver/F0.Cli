using System;
using System.Reflection;
using F0.Cli;
using F0.Hosting;
using F0.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace F0.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCli(this IServiceCollection services, Assembly commandAssembly, string[] args)
		{
			_ = commandAssembly ?? throw new ArgumentNullException(nameof(commandAssembly));
			_ = args ?? throw new ArgumentNullException(nameof(args));

			services.Configure<ConsoleLifetimeOptions>(static options =>
			{
				options.SuppressStatusMessages = true;
			});

			services.AddSingleton<IReporter, ConsoleReporter>();
			services.AddSingleton(sp => new CommandContext(args, commandAssembly));

			services.AddHostedService<CommandLineBackgroundService>();

			return services;
		}
	}
}
