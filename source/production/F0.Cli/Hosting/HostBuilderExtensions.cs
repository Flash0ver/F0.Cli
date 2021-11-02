using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using F0.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace F0.Hosting
{
	public static partial class HostBuilderExtensions
	{
		public static IHostBuilder UseAssemblyAttributes(this IHostBuilder hostBuilder, Assembly assembly)
		{
			_ = assembly ?? throw new ArgumentNullException(nameof(assembly));

			return hostBuilder
				.UseEnvironment(assembly)
				.UseApplicationName(assembly);
		}

		public static IHostBuilder UseEnvironment(this IHostBuilder hostBuilder, Assembly assembly)
		{
			_ = assembly ?? throw new ArgumentNullException(nameof(assembly));

			AssemblyConfigurationAttribute? configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
			Debug.Assert(configuration is not null, $"{nameof(Assembly)} '{assembly}' does not contain expected attribute '{nameof(AssemblyConfigurationAttribute)}'.");

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
			_ = assembly ?? throw new ArgumentNullException(nameof(assembly));

			AssemblyProductAttribute? product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
			Debug.Assert(product is not null, $"{nameof(Assembly)} '{assembly}' does not contain expected attribute '{nameof(AssemblyProductAttribute)}'.");

			return hostBuilder.UseSetting(HostDefaults.ApplicationKey, product.Product);
		}

		public static IHostBuilder UseCli(this IHostBuilder hostBuilder, Assembly commandAssembly, string[] args)
		{
			_ = commandAssembly ?? throw new ArgumentNullException(nameof(commandAssembly));
			_ = args ?? throw new ArgumentNullException(nameof(args));

			return hostBuilder.ConfigureServices((hostingContext, services) =>
			{
				services.AddCli(commandAssembly, args);
			});
		}

		internal static IHostBuilder UseSetting(this IHostBuilder hostBuilder, string key, string value)
		{
			_ = key ?? throw new ArgumentNullException(nameof(key));
			_ = value ?? throw new ArgumentNullException(nameof(value));

			return hostBuilder.ConfigureHostConfiguration(configBuilder =>
			{
				configBuilder.AddInMemoryCollection(new[]
				{
					new KeyValuePair<string, string>(key, value)
				});
			});
		}
	}

	public static partial class HostBuilderExtensions
	{
		public static IHostBuilder UseAssemblyAttributes<TApplicationPart>(this IHostBuilder hostBuilder)
		{
			Assembly assembly = typeof(TApplicationPart).Assembly;

			return hostBuilder.UseAssemblyAttributes(assembly);
		}

		public static IHostBuilder UseEnvironment<TApplicationPart>(this IHostBuilder hostBuilder)
		{
			Assembly assembly = typeof(TApplicationPart).Assembly;

			return hostBuilder.UseEnvironment(assembly);
		}

		public static IHostBuilder UseApplicationName<TApplicationPart>(this IHostBuilder hostBuilder)
		{
			Assembly assembly = typeof(TApplicationPart).Assembly;

			return hostBuilder.UseApplicationName(assembly);
		}

		public static IHostBuilder UseCli<TApplicationPart>(this IHostBuilder hostBuilder, string[] args)
		{
			Assembly commandAssembly = typeof(TApplicationPart).Assembly;

			return hostBuilder.UseCli(commandAssembly, args);
		}
	}
}
