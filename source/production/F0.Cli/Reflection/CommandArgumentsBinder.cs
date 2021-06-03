using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F0.Cli;

namespace F0.Reflection
{
	internal static class CommandArgumentsBinder
	{
		internal static void BindArguments(CommandBase command, CommandLineArguments args)
		{
			_ = command ?? throw new ArgumentNullException(nameof(command));
			_ = args ?? throw new ArgumentNullException(nameof(args));

			if (args.HasArguments)
			{
				Type type = command.GetType();
				PropertyInfo[] candidates = GetArguments(type);
				PropertyInfo property = MatchArguments(candidates, command);
				SetArguments(property, command, args);
			}
		}

		private static PropertyInfo[] GetArguments(Type type)
		{
			PropertyInfo[] properties = type.GetProperties()
				.Where(static property =>
				{
					return property.Name.Equals("Args", StringComparison.Ordinal)
						|| property.Name.Equals("Arguments", StringComparison.Ordinal);
				})
				.ToArray();

			return properties;
		}

		private static PropertyInfo MatchArguments(PropertyInfo[] candidates, CommandBase command)
		{
			if (candidates.Length == 0)
			{
				throw new CommandArgumentsNotFoundException(command);
			}
			if (candidates.Length > 1)
			{
				throw new AmbiguousCommandArgumentsException(command, candidates);
			}

			return candidates.Single();
		}

		private static void SetArguments(PropertyInfo property, CommandBase command, CommandLineArguments args)
		{
			if (typeof(IEnumerable<string>).IsAssignableFrom(property.PropertyType))
			{
				if (property.PropertyType.IsArray)
				{
					property.SetValue(command, args.Arguments.ToArray());
				}
				else
				{
					property.SetValue(command, args.Arguments);
				}
			}
			else
			{
				throw new UnsupportedCommandArgumentsTypeException(property);
			}
		}
	}
}
