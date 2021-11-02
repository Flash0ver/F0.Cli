using System;
using System.Linq;
using System.Reflection;
using F0.Cli;
using F0.DependencyInjection;
using F0.Hosting;
using F0.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace F0.Tests.DependencyInjection
{
	public class ServiceCollectionExtensionsTests
	{
		private readonly Assembly assembly = typeof(ServiceCollectionExtensionsTests).Assembly;

		[Fact]
		public void NullChecks()
		{
			IServiceCollection services = new ServiceCollection();

			Assert.Throws<ArgumentNullException>("commandAssembly", () => ServiceCollectionExtensions.AddCli(services, null!, Array.Empty<string>()));
			Assert.Throws<ArgumentNullException>("args", () => ServiceCollectionExtensions.AddCli(services, assembly, null!));
		}

		[Fact]
		public void ApiIsFluent()
		{
			IServiceCollection services = new ServiceCollection();

			Assert.Same(services, services.AddCli(assembly, Array.Empty<string>()));
		}

		[Fact]
		public void ConfigureServices()
		{
			IServiceCollection services = new ServiceCollection();
			services.AddCli(assembly, Array.Empty<string>());

			ServiceDescriptor lifetime = services.Single(d => d.ServiceType == typeof(IConfigureOptions<ConsoleLifetimeOptions>));
			Assert.Null(lifetime.ImplementationType);
			Assert.Equal(ServiceLifetime.Singleton, lifetime.Lifetime);

			ServiceDescriptor reporter = services.Single(d => d.ServiceType == typeof(IReporter));
			Assert.Equal(typeof(ConsoleReporter), reporter.ImplementationType);
			Assert.Equal(ServiceLifetime.Singleton, reporter.Lifetime);

			ServiceDescriptor context = services.Single(d => d.ServiceType == typeof(CommandContext));
			Assert.Null(context.ImplementationType);
			Assert.Equal(ServiceLifetime.Singleton, context.Lifetime);

			ServiceDescriptor backgroundService = services.Single(d => d.ImplementationType == typeof(CommandLineBackgroundService));
			Assert.Equal(typeof(IHostedService), backgroundService.ServiceType);
			Assert.Equal(ServiceLifetime.Singleton, backgroundService.Lifetime);
		}
	}
}
