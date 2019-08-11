namespace F0.Cli
{
	public sealed class CommandResult
	{
		internal CommandResult()
		{
		}

		internal CommandResult(int exitCode)
		{
			ExitCode = exitCode;
		}

		public int ExitCode { get; }

		public bool IsSuccess => ExitCode == 0;
		public bool IsFailure => ExitCode != 0;
	}
}
