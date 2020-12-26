using System;
using F0.Cli;
using Microsoft.Extensions.DependencyInjection;

namespace F0.Reflection
{
	internal static class CommandActivator
	{
		internal static CommandBase ConstructCommand(IServiceProvider provider, Type type)
		{
			if (provider is null)
			{
				throw new ArgumentNullException(nameof(provider));
			}
			if (type is null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			CommandBase command = ActivateCommand(provider, type);

			if (command is null)
			{
				throw new ArgumentException($"{nameof(Type)} must be of type {typeof(CommandBase)}.", nameof(type));
			}

			return command;
		}

		private static CommandBase ActivateCommand(IServiceProvider provider, Type type)
		{
			return ActivatorUtilities.CreateInstance(provider, type) as CommandBase;
		}
	}
}
