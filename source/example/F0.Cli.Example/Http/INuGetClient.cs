using System.Threading;
using System.Threading.Tasks;

namespace F0.Cli.Example.Http
{
	public interface INuGetClient
	{
		Task<string> GetByOwnerAsync(string owner, CancellationToken cancellationToken);
		Task<string> GetByIdAsync(string id, CancellationToken cancellationToken);
	}
}
