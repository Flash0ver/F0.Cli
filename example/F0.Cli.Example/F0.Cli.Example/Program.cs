using System;
using F0.Cli.Example.Http;
using F0.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace F0.Cli.Example
{
	internal static class Program
	{
		private static int Main(string[] args)
		{
			Console.WriteLine("F0.Cli");
			Console.WriteLine();

			if (args.Length == 0)
			{
				args = new string[] { "package", "F0.Cli", "--author", "Flash0ver" };
			}

			Console.WriteLine($"command-line arguments {args.Length}: {String.Join(' ', args)}");

			int exitCode = CreateHostBuilder(args).Build().RunCli();

			Console.WriteLine();
			string consoleAppName = AppDomain.CurrentDomain.FriendlyName;
			string result = $"{exitCode} (0x{exitCode.ToString("x")})";
			Console.WriteLine($"The process '{consoleAppName}' has exited with code {result}.");

			return exitCode;
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			var builder = new HostBuilder();
			builder.UseAssemblyAttributes();
			builder.ConfigureServices((hostingContext, services) =>
			{
				services.AddHttpClient<INuGetClient, NuGetClient>();
			});
			return builder.UseCli(args);
		}
	}
}
