using System.Threading.Tasks;

namespace F0.Cli.Example.Http
{
	public interface INuGetClient
	{
		Task<string> GetByAuthorAsync(string author);
		Task<string> GetByIdAsync(string id);
	}
}
