using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

    const string Launcher = "https://raw.githubusercontent.com/flarialmc/newcdn/main/launcher/launcherVersion.txt";

    const string Store = "https://fe3cr.delivery.mp.microsoft.com/ClientWebService/client.asmx/secured";

    const string Supported = "https://raw.githubusercontent.com/flarialmc/newcdn/main/launcher/NewSupported.txt";

    static readonly HttpClient Client = new(new HttpClientHandler()
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        AllowAutoRedirect = true
    });

    internal static async Task DownloadAsync(string uri, string path, Action<int> action = default)
    {
        using var message = await Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead); message.EnsureSuccessStatusCode();
        using Stream source = await message.Content.ReadAsStreamAsync(), destination = File.Create(path);

        int @this = default; var @object = new byte[Environment.SystemPageSize];
        long @params = message.Content.Headers.ContentLength.GetValueOrDefault(), value = default;

        while ((@this = await source.ReadAsync(@object, default, @object.Length)) != default)
        {
            await destination.WriteAsync(@object, default, @this);
            if (action != default && @params != default) action((int)Math.Round(100F * (value += @this) / @params));
        }
    }

    internal static async Task<HashSet<string>> SupportedAsync()
    {
        HashSet<string> collection = [];
        using StreamReader stream = new(await Client.GetStreamAsync(Supported));

        string value = default;
        while ((value = await stream.ReadLineAsync()) != default)
            collection.Add(value);

        return collection;
    }

    internal static async Task<Stream> FrameworkAsync()
    {
        using var reader = JsonReaderWriterFactory.CreateJsonReader(await Client.GetStreamAsync(Index), XmlDictionaryReaderQuotas.Max);
        return await Client.GetStreamAsync(XElement.Load(reader).Descendants("packageContent").Last(_ => _.Value.StartsWith("https://")).Value);
    }

    internal static async Task<Uri> UriAsync(HttpContent content)
    {
        using var message = await Client.PostAsync(Store, content);
        using var stream = await message.Content.ReadAsStreamAsync();
        return new(XElement.Load(stream).Descendants().FirstOrDefault(_ => _.Value.StartsWith("http://tlu.dl.delivery.mp.microsoft.com", StringComparison.Ordinal)).Value);
    }

    internal static async Task<IEnumerable<string>> VersionsAsync() => JsonArray.Parse(await Client.GetStringAsync(Releases)).Select(_ => _.GetString());

    internal static async Task<string> HashAsync(bool value) => JsonObject.Parse(await Client.GetStringAsync(Hashes))[value ? "Beta" : "Release"].GetString();

    internal static async Task<JsonObject> LauncherAsync() => await Task.Run(async () => JsonObject.Parse(await Client.GetStringAsync(Launcher)))
;
}