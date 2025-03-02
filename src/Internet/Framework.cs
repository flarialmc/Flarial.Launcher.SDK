using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Flarial.Launcher.SDK;

static partial class Internet
{
    readonly static JavaScriptSerializer Serializer = new()
    {
        MaxJsonLength = int.MaxValue,
        RecursionLimit = int.MaxValue
    };

    internal static async Task<string> HashAsync(bool value) =>
    Serializer.Deserialize<Dictionary<string, string>>(await HttpClient.GetStringAsync(Hashes))[value ? "Beta" : "Release"];

    internal static async Task<IEnumerable<string>> PackagesAsync() =>
    await Task.Run(async () => Serializer.Deserialize<string[]>(await HttpClient.GetStringAsync(Packages)));
}