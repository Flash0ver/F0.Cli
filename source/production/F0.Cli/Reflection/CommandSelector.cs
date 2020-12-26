using System;
using System.Linq;
using System.Reflection;
using F0.Cli;

namespace F0.Reflection
{
	internal static class CommandSelector
	{
		internal static Type SelectCommand(Assembly assembly, CommandLineArguments args)
		{
			if (assembly is null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}
			if (args is null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			ILookup<string, Type> commands = GetCommands(assembly);
			Type command = MatchCommand(commands, args);
			return command;
		}

		private static ILookup<string, Type> GetCommands(Assembly assembly)
		{
			string convention = "Command";

			ILookup<string, Type> commands = assembly.GetTypes()
				.Where(type => type.IsPublic && !type.IsAbstract && typeof(CommandBase).IsAssignableFrom(type))
				.ToLookup(type =>
				{
					string name = type.Name;

					if (name.EndsWith(convention, StringComparison.OrdinalIgnoreCase) &&
						!name.Equals(convention, StringComparison.OrdinalIgnoreCase))
					{
						name = name.Substring(0, name.LastIndexOf(convention, StringComparison.OrdinalIgnoreCase));
					}

					return name.ToLowerInvariant();
				});

			return commands;
		}

		private static Type MatchCommand(ILookup<string, Type> commands, CommandLineArguments args)
		{
			if (args.Verb.Length == 0)
			{
				throw new CommandNotProvidedException();
			}

			Type[] candidates = commands[args.Verb].ToArray();

			if (candidates.Length == 0)
			{
				throw new CommandNotFoundException(args.Verb);
			}
			if (candidates.Length > 1)
			{
				throw new AmbiguousCommandException(candidates, args.Verb);
			}

			return candidates.Single();
		}
	}
}
