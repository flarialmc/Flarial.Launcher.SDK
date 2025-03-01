using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Flarial.Launcher.SDK;

internal static class Web
{
    const string Hashes = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll_hashes.json";

    const string Packages = "https://raw.githubusercontent.com/dummydummy123456/BedrockDB/main/releases.json";

    internal static async Task<string> HashAsync(bool value) =>
    (await JsonNode.ParseAsync(await Shared.HttpClient.GetStreamAsync(Hashes)))[value ? "Beta" : "Release"].GetValue<string>();

    internal static async Task<IEnumerable<string>> PackagesAsync() =>
    (await JsonNode.ParseAsync(await Shared.HttpClient.GetStreamAsync(Packages ))).AsArray().Select(_ => _.GetValue<string>());
}