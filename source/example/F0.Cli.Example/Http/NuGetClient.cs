using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

		async Task<string> INuGetClient.GetByOwnerAsync(string owner, CancellationToken cancellationToken)
		{
			using HttpResponseMessage response = await client.GetAsync($"query?q=owner:{owner}", cancellationToken);
			using Stream json = await response.Content.ReadAsStreamAsync();
			using JsonDocument document = await JsonDocument.ParseAsync(json, default, cancellationToken);
			JsonElement root = document.RootElement;

			var text = new StringBuilder();

			int totalHits = root.GetProperty("totalHits").GetInt32();
			text.AppendLine($"{totalHits} packages by {owner}:");

			int downloads = 0;
			JsonElement.ArrayEnumerator data = root.GetProperty("data").EnumerateArray();
			foreach (JsonElement package in data)
			{
				string id = package.GetProperty("id").GetString();
				string type = package.GetProperty("@type").GetString();
				int totalDownloads = package.GetProperty("totalDownloads").GetInt32();

				text.AppendLine($"* {id} ({type}) - {totalDownloads} downloads");
				downloads += totalDownloads;
			}
			text.Append($"Total downloads of packages: {downloads}");

			return text.ToString();
		}

		async Task<string> INuGetClient.GetByIdAsync(string id, CancellationToken cancellationToken)
		{
			using HttpResponseMessage response = await client.GetAsync($"query?q=PackageId:{id}", cancellationToken);
			using Stream json = await response.Content.ReadAsStreamAsync();
			using JsonDocument document = await JsonDocument.ParseAsync(json, default, cancellationToken);
			JsonElement root = document.RootElement;

			var text = new StringBuilder();

			int totalHits = root.GetProperty("totalHits").GetInt32();
			if (totalHits != 1)
			{
				string message = $"Package '{id}' not found.";
				throw new InvalidOperationException(message);
			}

			JsonElement.ArrayEnumerator data = root.GetProperty("data").EnumerateArray();
			JsonElement package = data.Single();

			string title = package.GetProperty("title").GetString();
			string type = package.GetProperty("@type").GetString();
			JsonElement.ArrayEnumerator tags = package.GetProperty("tags").EnumerateArray();
			string description = package.GetProperty("description").GetString();
			text.Append($"{title} ({type}) [{String.Join(", ", tags)}] | {description}");

			JsonElement.ArrayEnumerator versions = package.GetProperty("versions").EnumerateArray();
			foreach (JsonElement version in versions)
			{
				string v = version.GetProperty("version").GetString();
				int downloads = version.GetProperty("downloads").GetInt32();

				text.AppendLine();
				text.Append($"  - {v} / {downloads} downloads");
			}

			return text.ToString();
		}

		async Task<string> INuGetClient.GetByTagAsync(string tag, int skip, int take, CancellationToken cancellationToken)
		{
			using HttpResponseMessage response = await client.GetAsync(BuildSearchQuery($"tag:{tag}", skip, take), cancellationToken);
			using Stream json = await response.Content.ReadAsStreamAsync();
			using JsonDocument document = await JsonDocument.ParseAsync(json, default, cancellationToken);
			JsonElement root = document.RootElement;

			var text = new StringBuilder();

			int totalHits = root.GetProperty("totalHits").GetInt32();
			text.AppendLine($"{totalHits} packages tagged {tag}:");

			JsonElement data = root.GetProperty("data");
			int count = data.GetArrayLength();

			for (int i = 0; i < count; i++)
			{
				JsonElement package = data[i];
				string id = package.GetProperty("id").GetString();
				string version = package.GetProperty("version").GetString();
				string authors = String.Join(" & ", package.GetProperty("authors").EnumerateArray());
				text.AppendLine($"* {id} (v{version}) - by {authors}");
			}

			text.AppendLine($"skip {skip} + take {take} = {count} packages");

			return text.ToString();
		}

		void IDisposable.Dispose()
		{
			client.Dispose();
		}

		private static string BuildSearchQuery(string q, int skip, int take)
		{
			var query = new StringBuilder($"query?q={q}");

			if (skip > 0)
			{
				query.Append($"&skip={skip}");
			}

			if (take > 0)
			{
				query.Append($"&take={take}");
			}

			return query.ToString();
		}
	}
}
