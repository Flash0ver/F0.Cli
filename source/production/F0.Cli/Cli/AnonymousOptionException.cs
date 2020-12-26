using System;

namespace F0.Cli
{
	internal sealed class AnonymousOptionException : Exception
	{
		public AnonymousOptionException()
			: base(CreateMessage())
		{
		}

		private static string CreateMessage()
		{
			string message = $"Options require a name.";
			return message;
		}
	}
}
