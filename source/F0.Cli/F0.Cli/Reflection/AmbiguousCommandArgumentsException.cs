using System;
using System.Linq;
using System.Reflection;
using F0.Cli;

namespace F0.Reflection
{
	internal sealed class AmbiguousCommandArgumentsException : Exception
	{
		public AmbiguousCommandArgumentsException(CommandBase command, PropertyInfo[] candidates)
			 : base(CreateMessage(command, candidates))
		{
		}

		private static string CreateMessage(CommandBase command, PropertyInfo[] candidates)
		{
			Type type = command.GetType();
			string ambiguities = String.Join(Environment.NewLine, candidates.Select(candidate => $"  {candidate.Name}"));

			string message = $"Ambiguous arguments convention used by '{type}'. Multiple arguments matched:";
			message += $"{Environment.NewLine}{ambiguities}";
			return message;
		}
	}
}
