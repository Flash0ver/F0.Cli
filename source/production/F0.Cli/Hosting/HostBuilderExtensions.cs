using System;
using System.Collections.Generic;
using System.Reflection;
using F0.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace F0.Hosting
{
	public static class HostBuilderExtensions
	{
		public static IHostBuilder UseAssemblyAttributes(this IHostBuilder hostBuilder)
		{
			Assembly assembly = Assembly.GetEntryAssembly();

			return hostBuilder
				.UseEnvironment(assembly)
				.UseApplicationName(assembly);
		}

		public static IHostBuilder UseEnvironment(this IHostBuilder hostBuilder, Assembly assembly)
		{
			AssemblyConfigurationAttribute configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();

			string environment = configuration.Configuration switch
			{
				"Debug" => Environments.Development,
				"Release" => Environments.Production,
				_ => Environments.Staging,
			};

			return hostBuilder.UseEnvironment(environment);
		}

		public static IHostBuilder UseApplicationName(this IHostBuilder hostBuilder, Assembly assembly)
		{
			AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();

			return hostBuilder.UseSetting(HostDefaults.ApplicationKey, product.Product);
		}

		public static IHostBuilder UseCli(this IHostBuilder hostBuilder, string[] args)
		{
			return hostBuilder.ConfigureServices((hostingContext, services) =>
			{
				services.AddCli(args);
			});
		}

		internal static IHostBuilder UseSetting(this IHostBuilder hostBuilder, string key, string value)
		{
			if (key is null)
			{
				throw new ArgumentNullException(nameof(key));
			}
			if (value is null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			return hostBuilder.ConfigureHostConfiguration(configBuilder =>
			{
				configBuilder.AddInMemoryCollection(new[]
				{
					new KeyValuePair<string, string>(key, value)
				});
			});
		}
	}
}
