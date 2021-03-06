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
			Assert.Same(hostBuilder, hostBuilder.UseAssemblyAttributes());

			Assembly assembly = Assembly.GetEntryAssembly();
			IHost host = hostBuilder.Build();
			IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();

			AssemblyConfigurationAttribute configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
			Assert.Equal("", configuration.Configuration);
			Assert.Equal(Environments.Staging, environment.EnvironmentName);

			AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
			Assert.Contains("Test", product.Product);
			Assert.Equal(product.Product, environment.ApplicationName);
		}

		[Fact]
		public void ConfigureEnvironmentFromAssemblyConfiguration()
		{
			IHostBuilder hostBuilder = new HostBuilder();

			Assembly assembly = typeof(HostBuilderExtensionsTests).Assembly;
			Assert.Same(hostBuilder, hostBuilder.UseEnvironment(assembly));

			IHost host = hostBuilder.Build();

			AssemblyConfigurationAttribute configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
			Assert.True(configuration.Configuration == "Debug" || configuration.Configuration == "Release");

			IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();
			string environmentName = configuration.Configuration == "Debug" ? Environments.Development : Environments.Production;
			Assert.Equal(environmentName, environment.EnvironmentName);
		}

		[Fact]
		public void ConfigureApplicationFromAssemblyProduct()
		{
			IHostBuilder hostBuilder = new HostBuilder();

			Assembly assembly = typeof(HostBuilderExtensionsTests).Assembly;
			Assert.Same(hostBuilder, hostBuilder.UseApplicationName(assembly));

			IHost host = hostBuilder.Build();

			AssemblyProductAttribute product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
			Assert.Equal("F0.Cli.Tests", product.Product);

			IHostEnvironment environment = host.Services.GetRequiredService<IHostEnvironment>();
			Assert.Equal("F0.Cli.Tests", environment.ApplicationName);
		}

		[Fact]
		public void UseCli()
		{
			IHostBuilder hostBuilder = new HostBuilder();
			Assert.Same(hostBuilder, hostBuilder.UseCli(new string[] { "F0.Cli" }));
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
			Assert.Same(hostBuilder, hostBuilder.UseCli(Array.Empty<string>()));
			IHost host = hostBuilder.Build();

			TimeSpan timeout = new HostOptions().ShutdownTimeout;
			IOptions<HostOptions> options = host.Services.GetRequiredService<IOptions<HostOptions>>();
			Assert.Equal(timeout, options.Value.ShutdownTimeout);
		}

		[Fact]
		public void UseSetting()
		{
			IHostBuilder hostBuilder = new HostBuilder();

			Assert.Throws<ArgumentNullException>("key", () => hostBuilder.UseSetting(null, ""));
			Assert.Throws<ArgumentNullException>("value", () => hostBuilder.UseSetting("", null));

			Assert.Same(hostBuilder, hostBuilder.UseSetting("Key", "Value"));

			IHost host = hostBuilder.Build();
			IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();

			Assert.Equal("Value", configuration["Key"]);
		}
	}
}
