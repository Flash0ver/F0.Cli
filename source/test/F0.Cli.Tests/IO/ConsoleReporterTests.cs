using System;
using System.IO;
using System.Linq;
using System.Reflection;
using F0.IO;
using Xunit;

namespace F0.Tests.IO
{
	[Collection(nameof(Console))]
	public class ConsoleReporterTests : IDisposable
	{
		private readonly TextWriter writer;

		public ConsoleReporterTests()
		{
			writer = new StringWriter();
		}

		void IDisposable.Dispose()
		{
			writer.Dispose();
		}

		[Fact]
		public void Check_Fixture()
		{
			Assert.Equal(Environment.NewLine, writer.NewLine);
		}

		[Fact]
		public void DefaultConstructorIsPublic()
		{
			ConstructorInfo constructor = typeof(ConsoleReporter).GetConstructors().Single();

			Assert.True(constructor.IsPublic);
			Assert.Empty(constructor.GetParameters());
		}

		[Fact]
		public void DoesNotRedirectTheStandardOutputStream()
		{
			TextWriter before = Console.Out;
			_ = new ConsoleReporter();
			TextWriter after = Console.Out;

			Assert.Same(before, after);
		}

		[Fact]
		public void Write_EmptyNewLine()
		{
			IReporter reporter = CreateReporter(writer);

			Assert.Equal("", writer.ToString());
			reporter.WriteLine();
			Assert.Equal($"{Console.Out.NewLine}", writer.ToString());
		}

		[Fact]
		public void Write_Information()
		{
			IReporter reporter = CreateReporter(writer);

			Assert.Equal("", writer.ToString());
			reporter.WriteInfo("Information");
			Assert.Equal($"Information{Console.Out.NewLine}", writer.ToString());
		}

		[Fact]
		public void Write_Warning()
		{
			IReporter reporter = CreateReporter(writer);

			Assert.Equal("", writer.ToString());
			reporter.WriteWarning("Warning");
			Assert.Equal($"Warning{Console.Out.NewLine}", writer.ToString());
		}

		[Fact]
		public void Write_Error()
		{
			IReporter reporter = CreateReporter(writer);

			Assert.Equal("", writer.ToString());
			reporter.WriteError("Error");
			Assert.Equal($"Error{Console.Out.NewLine}", writer.ToString());
		}

		private static ConsoleReporter CreateReporter(TextWriter standardOutput)
		{
			ConsoleReporter reporter = new();
			Console.SetOut(standardOutput);
			return reporter;
		}
	}
}
