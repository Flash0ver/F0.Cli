using System;
using System.Collections.Generic;
using F0.IO;

namespace F0.Tests.IO
{
	internal sealed class TestReporter : IReporter
	{
		private readonly Queue<LogMessage> messages;

		public TestReporter()
		{
			messages = new Queue<LogMessage>();
		}

		void IReporter.WriteLine()
		{
			Write(LogSeverity.None, null);
		}

		void IReporter.WriteInfo(string message)
		{
			Write(LogSeverity.Info, message);
		}

		void IReporter.WriteWarning(string message)
		{
			Write(LogSeverity.Warning, message);
		}

		void IReporter.WriteError(string message)
		{
			Write(LogSeverity.Error, message);
		}

		internal void CheckNextHidden()
		{
			CheckNext(LogSeverity.None, null);
		}

		internal void CheckNextInfo(string message)
		{
			CheckNext(LogSeverity.Info, message);
		}

		internal void CheckNextWarning(string message)
		{
			CheckNext(LogSeverity.Warning, message);
		}

		internal void CheckNextError(string message)
		{
			CheckNext(LogSeverity.Error, message);
		}

		internal void CheckEmpty()
		{
			if (messages.Count != 0)
			{
				throw new InvalidOperationException($"{messages.Count} messages unchecked.");
			}
		}

		private void Write(LogSeverity severity, string message)
		{
			var log = new LogMessage(severity, message);
			messages.Enqueue(log);
		}

		private void CheckNext(LogSeverity severity, string message)
		{
			LogMessage log = messages.Dequeue();
			if (log.Severity != severity)
			{
				throw new InvalidOperationException($"Expected severity: {severity}{Environment.NewLine}Actual severity: {log.Severity}");
			}
			if (log.Message != message)
			{
				throw new InvalidOperationException($"Expected message: {message}{Environment.NewLine}Actual message: {log.Message}");
			}
		}
	}

	internal sealed class LogMessage
	{
		public LogMessage(LogSeverity severity, string message)
		{
			Severity = severity;
			Message = message;
		}

		public LogSeverity Severity { get; }
		public string Message { get; }
	}

	internal enum LogSeverity
	{
		None,
		Info,
		Warning,
		Error
	}
}
