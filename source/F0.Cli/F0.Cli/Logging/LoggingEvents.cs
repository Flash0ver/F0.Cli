namespace F0.Logging
{
	internal static class LoggingEvents
	{
		public const int CommandPipelineSuccess = 0x_00_0_0000;

		public const int CommandExecutionCanceled = 0x_F0_2_0000;

		public const int CommandPipelineFailure = 0x_F0_4_0000;
		public const int CommandExecutionFaulted = 0x_F0_4_0001;
	}
}
