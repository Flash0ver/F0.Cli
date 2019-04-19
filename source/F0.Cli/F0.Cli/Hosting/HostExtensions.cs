using F0.Cli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace F0.Hosting
{
	public static class HostExtensions
	{
		public static int RunCli(this IHost host)
		{
			CommandContext context = host.Services.GetRequiredService<CommandContext>();
			host.Run();
			CommandResult result = context.GetResult();
			return result.ExitCode;
		}
	}
}
