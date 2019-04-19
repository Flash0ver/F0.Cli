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
		private readonly IReporter reporter;
		private readonly TextWriter writer;

		public ConsoleReporterTests()
		{
			reporter = new ConsoleReporter();

			Check_That_UseSystemConsole();

			writer = new StringWriter();
			Console.SetOut(writer);
		}

		private static void Check_That_UseSystemConsole()
		{
			Assert.False(Console.IsOutputRedirected);
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
		public void Write_EmptyNewLine()
		{
			Assert.Equal("", writer.ToString());
			reporter.WriteLine();
			Assert.Equal($"{Console.Out.NewLine}", writer.ToString());
		}

		[Fact]
		public void Write_Information()
		{
			Assert.Equal("", writer.ToString());
			reporter.WriteInfo("Information");
			Assert.Equal($"Information{Console.Out.NewLine}", writer.ToString());
		}

		[Fact]
		public void Write_Warning()
		{
			Assert.Equal("", writer.ToString());
			reporter.WriteWarning("Warning");
			Assert.Equal($"Warning{Console.Out.NewLine}", writer.ToString());
		}

		[Fact]
		public void Write_Error()
		{
			Assert.Equal("", writer.ToString());
			reporter.WriteError("Error");
			Assert.Equal($"Error{Console.Out.NewLine}", writer.ToString());
		}
	}
}
