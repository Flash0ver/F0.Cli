namespace F0.IO
{
	public interface IReporter
	{
		void WriteLine();
		void WriteInfo(string message);
		void WriteWarning(string message);
		void WriteError(string message);
	}
}
