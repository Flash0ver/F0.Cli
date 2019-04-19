using System;
using System.Collections.Generic;
using System.Reflection;
using F0.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace F0.Tests.Logging
{
	public class LoggingEventsTests
	{
		[Fact]
		public void EventId_IsUnique()
		{
			var set = new HashSet<int>();
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
				new LoggingEventsData(LoggingEvents.CommandPipelineFailure, LogLevel.Error),
				new LoggingEventsData(LoggingEvents.CommandExecutionFailed, LogLevel.Error)
			};
		}

		public static IEnumerable<object[]> GetData(bool includeLogLevel)
		{
			foreach (LoggingEventsData data in Data)
			{
				if (includeLogLevel)
				{
					yield return new object[] { data.Id, data.LogLevel };
				}
				else
				{
					yield return new object[] { data.Id };
				}
			}
		}

		public LoggingEventsData(int id, LogLevel logLevel)
		{
			Id = id;
			LogLevel = logLevel;
		}

		public int Id { get; }
		public LogLevel LogLevel { get; }
	}
}
