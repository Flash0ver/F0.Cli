using System;
using F0.Cli;
using Microsoft.Extensions.DependencyInjection;

namespace F0.Reflection
{
	internal static class CommandActivator
	{
		internal static CommandBase ConstructCommand(IServiceProvider provider, Type type)
		{
			_ = provider ?? throw new ArgumentNullException(nameof(provider));
			_ = type ?? throw new ArgumentNullException(nameof(type));

			CommandBase? command = ActivateCommand(provider, type);

			return command ?? throw new ArgumentException($"{nameof(Type)} must be of type {typeof(CommandBase)}.", nameof(type));
		}

		private static CommandBase? ActivateCommand(IServiceProvider provider, Type type)
		{
			return ActivatorUtilities.CreateInstance(provider, type) as CommandBase;
		}
	}
}
