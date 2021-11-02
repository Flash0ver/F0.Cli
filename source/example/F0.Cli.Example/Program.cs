using System;
using System.Globalization;
using System.Threading.Tasks;
using F0.Cli.Example.Http;
using F0.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace F0.Cli.Example
{
	internal static class Program
	{
		private static async Task<int> Main(string[] args)
		{
			Console.WriteLine("F0.Cli");
			Console.WriteLine();

			if (args.Length == 0)
			{
				args = new string[] { "package", "F0.Cli", "--owner", "Flash0ver", "--tag", "hosting", "--skip", "3", "--take", "3" };
			}

			Console.WriteLine($"command-line arguments {args.Length}: {String.Join(' ', args)}");

			int exitCode = await CreateHostBuilder(args).Build().RunCliAsync();

			Console.WriteLine();
			string consoleAppName = AppDomain.CurrentDomain.FriendlyName;
			string result = $"{exitCode} (0x{exitCode.ToString("x", NumberFormatInfo.InvariantInfo)})";
			Console.WriteLine($"The process '{consoleAppName}' has exited with code {result}.");

			return exitCode;
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			HostBuilder builder = new();
			builder.UseAssemblyAttributes<App>();
			builder.ConfigureServices(static (hostContext, services) =>
			{
				services.AddHttpClient<INuGetClient, NuGetClient>();
			});
			return builder.UseCli<App>(args);
		}

		private sealed class App
		{
		}
	}
}
