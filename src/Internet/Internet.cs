using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Flarial.Launcher.SDK;

static partial class Internet
{
    const string Hashes = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll_hashes.json";

    const string Packages = "https://raw.githubusercontent.com/dummydummy123456/BedrockDB/main/releases.json";

    const string Framework = "https://api.nuget.org/v3/registration5-gz-semver2/microsoft.services.store.engagement/index.json";

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

    internal static async Task<string> GetAsync(string value) => await HttpClient.GetStringAsync(value);

    internal static async Task<HttpResponseMessage> PostAsync(string value, HttpContent content) => await HttpClient.PostAsync(value, content);

    internal static async Task<Stream> FrameworkAsync()
    {
        using var reader = JsonReaderWriterFactory.CreateJsonReader(await HttpClient.GetStreamAsync(Framework), XmlDictionaryReaderQuotas.Max);
        return await HttpClient.GetStreamAsync(XElement.Load(reader).Descendants("packageContent").Last(_ => _.Value.StartsWith("https://")).Value);
    }
}