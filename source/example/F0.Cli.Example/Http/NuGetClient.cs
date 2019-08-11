using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace F0.Cli.Example.Http
{
	internal sealed class NuGetClient : INuGetClient, IDisposable
	{
		private readonly HttpClient client;

		public NuGetClient(HttpClient client)
		{
			client.BaseAddress = new Uri("https://api-v2v3search-0.nuget.org/");

			this.client = client;
		}

		async Task<string> INuGetClient.GetByAuthorAsync(string author, CancellationToken cancellationToken)
		{
			JObject json;

			using (HttpResponseMessage response = await client.GetAsync($"query?q=author:{author}", cancellationToken))
			{
				json = await response.Content.ReadAsAsync<JObject>(cancellationToken);
			}

			var text = new StringBuilder();

			text.AppendLine($"{json["totalHits"]} packages by {author}:");

			int downloads = 0;
			foreach (JToken package in json["data"])
			{
				text.AppendLine($"* {package["id"]} ({package["@type"]}) - {package["totalDownloads"]} downloads");
				downloads += Int32.Parse(package["totalDownloads"].ToString());
			}
			text.Append($"Total downloads of packages: {downloads}");

			return text.ToString();
		}

		async Task<string> INuGetClient.GetByIdAsync(string id, CancellationToken cancellationToken)
		{
			JObject json;

			using (HttpResponseMessage response = await client.GetAsync($"query?q=PackageId:{id}", cancellationToken))
			{
				json = await response.Content.ReadAsAsync<JObject>(cancellationToken);
			}

			JToken package = json["data"].Single();

			var text = new StringBuilder();

			text.Append($"{package["title"]} ({package["@type"]}) [{String.Join(", ", package["tags"])}]");
			text.Append($" | {package["description"]}");
			foreach (JToken version in package["versions"])
			{
				text.AppendLine();
				text.Append($"  - {version["version"]} / {version["downloads"]} downloads");
			}

			return text.ToString();
		}

		void IDisposable.Dispose()
		{
			client.Dispose();
		}
	}
}
