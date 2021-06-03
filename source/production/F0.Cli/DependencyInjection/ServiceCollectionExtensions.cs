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
		public static IServiceCollection AddCli(this IServiceCollection services, string[] args)
		{
			services.Configure<ConsoleLifetimeOptions>(static options =>
			{
				options.SuppressStatusMessages = true;
			});

			services.AddSingleton<IReporter, ConsoleReporter>();
			services.AddSingleton(sp => new CommandContext(args, Assembly.GetEntryAssembly()));

			services.AddHostedService<CommandLineBackgroundService>();

			return services;
		}
	}
}
