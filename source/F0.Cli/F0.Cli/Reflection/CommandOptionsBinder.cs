﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F0.Cli;

namespace F0.Reflection
{
	internal static class CommandOptionsBinder
	{
		internal static void BindOptions(CommandBase command, CommandLineArguments args)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}
			if (args is null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			IReadOnlyDictionary<string, PropertyInfo> candidates = GetOptions(command);
			SetOptions(candidates, command, args);
		}

		private static IReadOnlyDictionary<string, PropertyInfo> GetOptions(CommandBase command)
		{
			Type type = command.GetType();
			Dictionary<string, PropertyInfo> candidates = type.GetProperties().ToDictionary(property => property.Name.ToLowerInvariant());
			return candidates;
		}

		private static void SetOptions(IReadOnlyDictionary<string, PropertyInfo> candidates, CommandBase command, CommandLineArguments args)
		{
			foreach (KeyValuePair<string, string> option in args.Options)
			{
				PropertyInfo property = MatchOptions(candidates, command, option.Key);
				SetOption(property, command, option.Value);
			}
		}

		private static PropertyInfo MatchOptions(IReadOnlyDictionary<string, PropertyInfo> candidates, CommandBase command, string option)
		{
			PropertyInfo property = candidates.SingleOrDefault(prop =>
			{
				return prop.Key.Equals(option, StringComparison.Ordinal);
			}).Value;

			if (property is null)
			{
				throw new CommandOptionNotFoundException(command, option);
			}

			return property;
		}

		private static void SetOption(PropertyInfo property, CommandBase command, string value)
		{
			if (value is null)
			{
				if (typeof(bool).IsAssignableFrom(property.PropertyType))
				{
					property.SetValue(command, true);
				}
				else
				{
					throw new InvalidCommandSwitchException(property);
				}
			}
			else
			{
				if (typeof(string).IsAssignableFrom(property.PropertyType))
				{
					property.SetValue(command, value);
				}
				else
				{
					throw new UnsupportedCommandOptionTypeException(property);
				}
			}
		}
	}
}
