using System;
using System.Collections.ObjectModel;
using System.Reflection;
using F0.Cli;
using Xunit;

namespace F0.Tests.Cli
{
	public class CommandContextTests
	{
		private readonly Assembly assembly = typeof(CommandContextTests).Assembly;

		[Fact]
		public void HasReadonlyArgs()
		{
			var context = new CommandContext(new string[] { "F0.Cli" }, assembly);
			Assert.Equal(new[] { "F0.Cli" }, context.CommandLineArgs);
			Assert.IsType<ReadOnlyCollection<string>>(context.CommandLineArgs);
		}

		[Fact]
		public void ArgsMustNotBeNull()
		{
			Assert.Throws<ArgumentNullException>("args", () => new CommandContext(null, assembly));
		}

		[Fact]
		public void HasApplicationPartsAsAssembly()
		{
			var context = new CommandContext(Array.Empty<string>(), assembly);
			Assert.Same(assembly, context.CommandAssembly);
		}

		[Fact]
		public void AssemblyPartMustNotBeNull()
		{
			Assert.Throws<ArgumentNullException>("commandAssembly", () => new CommandContext(Array.Empty<string>(), null));
		}

		[Fact]
		public void ContextStoresResult()
		{
			var context = new CommandContext(Array.Empty<string>(), assembly);

			var result = new CommandResult(0);
			context.SetResult(result);
			Assert.Same(result, context.GetResult());
		}

		[Fact]
		public void ResultMustNotBeNull()
		{
			var context = new CommandContext(Array.Empty<string>(), assembly);

			Assert.Throws<InvalidOperationException>(() => context.GetResult());
			Assert.Throws<ArgumentNullException>("result", () => context.SetResult(null));
		}

		[Fact]
		public void ResultCanBeSetOnlyOnce()
		{
			var context = new CommandContext(Array.Empty<string>(), assembly);

			context.SetResult(new CommandResult(0));
			Assert.Throws<InvalidOperationException>(() => context.SetResult(new CommandResult(0)));
		}
	}
}
