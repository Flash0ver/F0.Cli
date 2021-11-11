using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F0.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace F0.Tests.Logging
{
	public class LoggingEventsTests
	{
		[Fact]
		public void EventId_IsUniqueErrorLevel()
		{
			ISet<int> set = new HashSet<int>();
			foreach (LoggingEventsData data in LoggingEventsData.Data)
			{
				Assert.True(set.Add(data.Id), $"Duplicate: {data.Id:X}");
			}

			BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			FieldInfo[] fields = typeof(LoggingEvents).GetFields(bindingFlags);
			Assert.Equal(set.Count, fields.Length);
		}

		[Theory]
		[MemberData(nameof(LoggingEventsData.GetData), false, MemberType = typeof(LoggingEventsData))]
		public void EventId_HasNamespace(int id)
		{
			EventId eventId = id;
			string hex = eventId.Id.ToString("X");
			Assert.Equal("F0", hex.Substring(0, 2));
		}

		[Theory]
		[MemberData(nameof(LoggingEventsData.GetData), true, MemberType = typeof(LoggingEventsData))]
		public void EventId_HasLogLevel(int id, LogLevel logLevel)
		{
			EventId eventId = id;
			string hex = eventId.Id.ToString("X");
			Assert.Equal(logLevel, (LogLevel)Int32.Parse(hex.Substring(2, 1)));
		}

		[Theory]
		[MemberData(nameof(LoggingEventsData.GetData), false, MemberType = typeof(LoggingEventsData))]
		public void EventId_IsNonNegativeExitCode(int id)
		{
			Assert.True(id >= 0, $"Negative exit code: {id:X}");
		}
	}

	internal sealed class LoggingEventsData
	{
		internal static IEnumerable<LoggingEventsData> Data = CreateData();

		private static LoggingEventsData[] CreateData()
		{
			return new LoggingEventsData[]
			{
				new LoggingEventsData(LoggingEvents.CommandPipelineSuccess, LogLevel.Trace),
				new LoggingEventsData(LoggingEvents.CommandExecutionCanceled, LogLevel.Information),
				new LoggingEventsData(LoggingEvents.CommandPipelineFailure, LogLevel.Error),
				new LoggingEventsData(LoggingEvents.CommandExecutionFaulted, LogLevel.Error),
			};
		}

		public static IEnumerable<object[]> GetData(bool includeLogLevel)
		{
			foreach (LoggingEventsData data in Data.Where(d => d.Id != 0))
			{
				yield return includeLogLevel
					? new object[] { data.Id, data.LogLevel }
					: new object[] { data.Id };
			}
		}

		public LoggingEventsData(int id, LogLevel logLevel)
		{
			(Id, LogLevel) = (id, logLevel);
		}

		public int Id { get; }
		public LogLevel LogLevel { get; }
	}
}
