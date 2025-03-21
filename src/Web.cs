using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Data.Json;

namespace Flarial.Launcher.SDK;

static partial class Web
{
    const string Hashes = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll_hashes.json";

    const string Releases = "https://raw.githubusercontent.com/dummydummy123456/BedrockDB/main/releases.json";

    const string Index = "https://api.nuget.org/v3/registration5-gz-semver2/microsoft.services.store.engagement/index.json";

    const string Launcher = "https://raw.githubusercontent.com/flarialmc/newcdn/refs/heads/main/launcher/launcherVersion.txt";

    const string Store = "https://fe3cr.delivery.mp.microsoft.com/ClientWebService/client.asmx/secured";

    const string Supported = "https://raw.githubusercontent.com/flarialmc/newcdn/main/launcher/NewSupported.txt";

    static readonly HttpClient HttpClient = new();

    internal static async Task DownloadAsync(string uri, string path, Action<int> action = default)
    {
        using var message = await HttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        message.EnsureSuccessStatusCode();

        using var stream = await message.Content.ReadAsStreamAsync();
        using FileStream destination = new(path, FileMode.Create, FileAccess.Write, FileShare.None, Environment.SystemPageSize, true);

        var count = 0; var value = 0L; var buffer = new byte[Environment.SystemPageSize];
        while ((count = await stream.ReadAsync(buffer, default, buffer.Length)) != default)
        {
            await destination.WriteAsync(buffer, default, count);
            if (action is not null) action((int)Math.Round(100F * (value += count) / message.Content.Headers.ContentLength.Value));
        }
    }

    internal static async Task<HashSet<string>> SupportedAsync()
    {
        HashSet<string> collection = [];
        using StreamReader stream = new(await HttpClient.GetStreamAsync(Supported));

        string value = default;
        while ((value = await stream.ReadLineAsync()) != default)
            collection.Add(value);

        return collection;
    }

    internal static async Task<Stream> FrameworkAsync()
    {
        using var reader = JsonReaderWriterFactory.CreateJsonReader(await HttpClient.GetStreamAsync(Index), XmlDictionaryReaderQuotas.Max);
        return await HttpClient.GetStreamAsync(XElement.Load(reader).Descendants("packageContent").Last(_ => _.Value.StartsWith("https://")).Value);
    }

    internal static async Task<Uri> UriAsync(HttpContent content)
    {
        using var message = await HttpClient.PostAsync(Store, content);
        using var stream = await message.Content.ReadAsStreamAsync();
        return new(XElement.Load(stream).Descendants().FirstOrDefault(_ => _.Value.StartsWith("http://tlu.dl.delivery.mp.microsoft.com", StringComparison.Ordinal)).Value);
    }

    internal static async Task<IEnumerable<string>> VersionsAsync() => JsonArray.Parse(await HttpClient.GetStringAsync(Releases)).Select(_ => _.GetString());

    internal static async Task<string> HashAsync(bool value) => JsonObject.Parse(await HttpClient.GetStringAsync(Hashes))[value ? "Beta" : "Release"].GetString();

    internal static async Task<JsonObject> LauncherAsync() => await Task.Run(async () => JsonObject.Parse(await HttpClient.GetStringAsync(Launcher)))
;
}