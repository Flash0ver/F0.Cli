using System;

namespace F0.IO
{
	internal sealed class ConsoleReporter : IReporter
	{
		public ConsoleReporter()
		{
		}

		void IReporter.WriteLine()
		{
			Console.WriteLine();
		}

		void IReporter.WriteInfo(string message)
		{
			Console.WriteLine(message);
		}

		void IReporter.WriteWarning(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		void IReporter.WriteError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
		}
	}
}
