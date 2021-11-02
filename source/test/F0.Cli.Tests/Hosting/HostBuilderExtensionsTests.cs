using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F0.Cli;
using F0.Hosting;
using F0.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace F0.Tests.Hosting
{
	public class HostBuilderExtensionsTests
	{
		[Fact]
		public void ConfigureFromAssemblyInfo()
		{
			IHostBuilder hostBuilder = new HostBuilder();

			Assert.Throws<ArgumentNullException>("assembly", () => HostBuilderExtensions.UseAssemblyAttributes(hostBuilder, null!));

			Assembly assembly = GetAssembly();
			Assert.Same(hostBuilder, hostBuilder.UseAssemblyAttributes(assembly));

			IHost host = hostBuilder.Build();
			IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();

			AssemblyConfigurationAttribute? configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
			Assert.NotNull(configuration);
			Assert.Equal(GetConfiguration(), configuration.Configuration);
			Assert.Equal(GetEnvironmentName(), environment.EnvironmentName);

			AssemblyProductAttribute? product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
			Assert.NotNull(product);
			Assert.Equal(GetApplicationName(), product.Product);
			Assert.Equal(product.Product, environment.ApplicationName);
		}

		[Fact]
		public void ConfigureEnvironmentFromAssemblyConfiguration()
		{
			IHostBuilder hostBuilder = new HostBuilder();

			Assert.Throws<ArgumentNullException>("assembly", () => HostBuilderExtensions.UseEnvironment(hostBuilder, null!));

			Assembly assembly = typeof(HostBuilderExtensionsTests).Assembly;
			Assert.Same(hostBuilder, hostBuilder.UseEnvironment(assembly));

			IHost host = hostBuilder.Build();

			AssemblyConfigurationAttribute? configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
			Assert.NotNull(configuration);
			Assert.True(configuration.Configuration is "Debug" or "Release");

			IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();
			string environmentName = configuration.Configuration == "Debug" ? Environments.Development : Environments.Production;
			Assert.Equal(environmentName, environment.EnvironmentName);
		}

		[Fact]
		public void ConfigureApplicationFromAssemblyProduct()
		{
			IHostBuilder hostBuilder = new HostBuilder();

			Assert.Throws<ArgumentNullException>("assembly", () => HostBuilderExtensions.UseApplicationName(hostBuilder, null!));

			Assembly assembly = typeof(HostBuilderExtensionsTests).Assembly;
			Assert.Same(hostBuilder, hostBuilder.UseApplicationName(assembly));

			IHost host = hostBuilder.Build();

			AssemblyProductAttribute? product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
			Assert.NotNull(product);
			Assert.Equal("F0.Cli.Tests", product.Product);

			IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();
			Assert.Equal("F0.Cli.Tests", environment.ApplicationName);
		}

		[Fact]
		public void UseCli()
		{
			IHostBuilder hostBuilder = new HostBuilder();
			Assembly assembly = typeof(HostBuilderExtensionsTests).Assembly;

			Assert.Throws<ArgumentNullException>("commandAssembly", () => HostBuilderExtensions.UseCli(hostBuilder, null!, Array.Empty<string>()));
			Assert.Throws<ArgumentNullException>("args", () => HostBuilderExtensions.UseCli(hostBuilder, assembly, null!));

			Assert.Same(hostBuilder, hostBuilder.UseCli(assembly, new string[] { "F0.Cli" }));
			IHost host = hostBuilder.Build();

			IOptions<ConsoleLifetimeOptions> lifetime = host.Services.GetRequiredService<IOptions<ConsoleLifetimeOptions>>();
			Assert.True(lifetime.Value.SuppressStatusMessages);

			IReporter reporter = host.Services.GetRequiredService<IReporter>();
			Assert.IsType<ConsoleReporter>(reporter);

			CommandContext context = host.Services.GetService<CommandContext>();
			Assert.Equal(new[] { "F0.Cli" }, context.CommandLineArgs);

			IEnumerable<IHostedService> services = host.Services.GetServices<IHostedService>();
			Assert.IsType<CommandLineBackgroundService>(services.Single());
		}

		[Fact]
		public void UseDefault_HostOptions_ShutdownTimeout()
		{
			IHostBuilder hostBuilder = new HostBuilder();
			Assembly assembly = typeof(HostBuilderExtensionsTests).Assembly;

			Assert.Same(hostBuilder, hostBuilder.UseCli(assembly, Array.Empty<string>()));
			IHost host = hostBuilder.Build();

			TimeSpan timeout = new HostOptions().ShutdownTimeout;
			IOptions<HostOptions> options = host.Services.GetRequiredService<IOptions<HostOptions>>();
			Assert.Equal(timeout, options.Value.ShutdownTimeout);
		}

		[Fact]
		public void UseSetting()
		{
			IHostBuilder hostBuilder = new HostBuilder();

			Assert.Throws<ArgumentNullException>("key", () => HostBuilderExtensions.UseSetting(hostBuilder, null!, ""));
			Assert.Throws<ArgumentNullException>("value", () => HostBuilderExtensions.UseSetting(hostBuilder, "", null!));

			Assert.Same(hostBuilder, hostBuilder.UseSetting("Key", "Value"));

			IHost host = hostBuilder.Build();
			IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();

			Assert.Equal("Value", configuration["Key"]);
		}

		private static Assembly GetAssembly()
		{
#if NETFRAMEWORK
			return typeof(HostBuilderExtensionsTests).Assembly;
#else
			Assembly? assembly = Assembly.GetEntryAssembly();
			Assert.NotNull(assembly);
			return assembly;
#endif
		}

		private static string GetConfiguration()
		{
#if !NETFRAMEWORK
			return String.Empty;
#elif DEBUG
			return "Debug";
#else
			return "Release";
#endif
		}

		private static string GetEnvironmentName()
		{
#if !NETFRAMEWORK
			return Environments.Staging;
#elif DEBUG
			return Environments.Development;
#else
			return Environments.Production;
#endif
		}

		private static string GetApplicationName()
		{
#if NETFRAMEWORK
			return typeof(HostBuilderExtensionsTests).Assembly.GetName().Name;
#else
			return "Microsoft.TestHost";
#endif
		}
	}
}
