using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Flarial.Launcher.SDK;

internal static partial class Internet
{
    internal static async Task<string> HashAsync(bool value) =>
    (await JsonNode.ParseAsync(await HttpClient.GetStreamAsync(Hashes)))[value ? "Beta" : "Release"].GetValue<string>();

    internal static async Task<IEnumerable<string>> PackagesAsync() =>
    (await JsonNode.ParseAsync(await HttpClient.GetStreamAsync(Packages))).AsArray().Select(_ => _.GetValue<string>());
}