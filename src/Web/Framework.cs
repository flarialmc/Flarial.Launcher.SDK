using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Flarial.Launcher.SDK;

internal static class Web
{
    readonly static JavaScriptSerializer Serializer = new()
    {
        MaxJsonLength = int.MaxValue,
        RecursionLimit = int.MaxValue
    };

    const string Packages = "https://raw.githubusercontent.com/dummydummy123456/BedrockDB/main/releases.json";

    const string Hashes = "https://raw.githubusercontent.com/flarialmc/newcdn/main/dll_hashes.json";

    internal static async Task<string> HashAsync(bool value) =>
    Serializer.Deserialize<Dictionary<string, string>>(await Shared.HttpClient.GetStringAsync(Hashes))[value ? "Beta" : "Release"];

    internal static async Task<IEnumerable<string>> PackagesAsync() =>
    await Task.Run(async () => Serializer.Deserialize<string[]>(await Shared.HttpClient.GetStringAsync(Packages)));
}