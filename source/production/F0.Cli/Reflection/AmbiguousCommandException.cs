using System;
using System.Linq;

namespace F0.Reflection
{
	internal sealed class AmbiguousCommandException : Exception
	{
		public AmbiguousCommandException(Type[] candidates, string verb)
			: base(CreateMessage(candidates, verb))
		{
		}

		private static string CreateMessage(Type[] candidates, string verb)
		{
			string ambiguities = String.Join(Environment.NewLine, candidates.Select(static candidate => candidate.FullName));

			string message = $"Ambiguous command '{verb}'. Multiple commands matched:";
			message += $"{Environment.NewLine}{ambiguities}";
			return message;
		}
	}
}
