using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ConsoleAppFramework;

var app = ConsoleApp.Create();
using var commands = new Commands(new());

app.Add("list", commands.ListAsync);
app.Add("view", commands.ViewAsync);
app.Add("new", commands.NewAsync);

app.Run(args);

sealed class Commands(HttpClient client) : IDisposable
{
    const string Ext = ".gitignore";

    public void Dispose()
    {
        client.Dispose();
    }

    /// <summary>
    /// List available repository gitignore templates
    /// </summary>
    public async Task ListAsync(CancellationToken ct = default)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://api.github.com/repos/github/gitignore/contents"),
            Headers = {
                { "User-Agent", "gitignore-cli" }
            }
        };

        var response = await client.SendAsync(request, ct);

        var typeInfo = (JsonTypeInfo<RepositoryContent[]>)SerializerContext.Default.Options.GetTypeInfo(typeof(RepositoryContent[]));
        var contents = await JsonSerializer.DeserializeAsync(response.Content.ReadAsStream(ct), typeInfo, ct);

        foreach (var content in contents!
            .Where(x => x.Name.EndsWith(Ext)))
        {
            Console.WriteLine(content.Name[..^Ext.Length]);
        }
    }

    /// <summary>
    /// View .gitignore file
    /// </summary>
    /// <param name="template">.gitignore template name</param>
    public async Task<int> ViewAsync([Argument] string template, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://raw.githubusercontent.com/github/gitignore/main/{template}.gitignore"),
        };

        var response = await client.SendAsync(request, ct);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            Console.WriteLine($".gitignore template '{template}' not found.");
            return 404;
        }

        Console.WriteLine(await response.Content.ReadAsStringAsync(ct));
        return response.IsSuccessStatusCode ? 0 : (int)response.StatusCode;
    }

    /// <summary>
    /// Create new .gitignore file
    /// </summary>
    /// <param name="template">.gitignore template name</param>
    /// <param name="output">-o, output path</param>
    public async Task<int> NewAsync([Argument] string template, string? output = null, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://raw.githubusercontent.com/github/gitignore/main/{template}.gitignore"),
        };

        var response = await client.SendAsync(request, ct);

        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            Console.WriteLine($".gitignore template '{template}' not found.");
            return 404;
        }

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync(ct));
            return (int)response.StatusCode;
        }

        await File.WriteAllBytesAsync(output ?? ".gitignore", await response.Content.ReadAsByteArrayAsync(ct), ct);
        return 0;
    }
}

[JsonSerializable(typeof(RepositoryContent))]
[JsonSerializable(typeof(RepositoryContent[]))]
sealed partial class SerializerContext : JsonSerializerContext
{
}

sealed record RepositoryContent
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}