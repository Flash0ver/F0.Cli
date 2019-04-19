using System;

namespace F0.Cli
{
	internal sealed class DuplicateOptionException : Exception
	{
		public DuplicateOptionException(string option)
			: base(CreateMessage(option))
		{
		}

		private static string CreateMessage(string option)
		{
			string message = $"Duplicate Switch: {option}.";
			return message;
		}
	}
}
