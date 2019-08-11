using System;
using F0.Cli;
using F0.Reflection;
using F0.Tests.Commands;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace F0.Tests.Reflection
{
	public class CommandActivatorTests
	{
		[Fact]
		public void NullCheck()
		{
			IServiceProvider provider = CreateDependencyInjection();

			Assert.Throws<ArgumentNullException>("provider", () => CommandActivator.ConstructCommand(null, typeof(NullCommand)));
			Assert.Throws<ArgumentNullException>("type", () => CommandActivator.ConstructCommand(provider, null));
		}

		[Fact]
		public void TypeMustBeCommand()
		{
			IServiceProvider provider = CreateDependencyInjection();

			Assert.Throws<ArgumentException>("type", () => CommandActivator.ConstructCommand(provider, typeof(CommandActivatorTests)));
		}

		[Fact]
		public void CreateCommandWithParameterlessConstructor()
		{
			IServiceProvider provider = CreateDependencyInjection();

			CommandBase command = CommandActivator.ConstructCommand(provider, typeof(NullCommand));

			Assert.IsType<NullCommand>(command);
		}

		[Fact]
		public void CreateCommandWithParameterizedConstructor()
		{
			Func<int> argument = () => 240;
			IServiceProvider provider = CreateDependencyInjection(argument);

			CommandBase command = CommandActivator.ConstructCommand(provider, typeof(DelegateCommand));

			Assert.IsType<DelegateCommand>(command);
		}

		[Fact]
		public void ActivatorDoesNotCatchExceptionThrownInCommandConstructor()
		{
			IServiceProvider provider = CreateDependencyInjection();

			Assert.Throws<NotSupportedException>(() => CommandActivator.ConstructCommand(provider, typeof(BadCommand)));
		}

		[Fact]
		public void ActivatorRequiresPublicConstructor()
		{
			IServiceProvider provider = CreateDependencyInjection();
			Type type = typeof(InvalidCommand);

			Assert.Empty(type.GetConstructors());
			Assert.Throws<InvalidOperationException>(() => CommandActivator.ConstructCommand(provider, type));
		}

		private static IServiceProvider CreateDependencyInjection(params object[] services)
		{
			IServiceCollection collection = new ServiceCollection();

			foreach (object service in services)
			{
				collection.AddSingleton(service.GetType(), service);
			}

			IServiceProviderFactory<IServiceCollection> factory = new DefaultServiceProviderFactory();

			return factory.CreateServiceProvider(collection);
		}
	}
}
