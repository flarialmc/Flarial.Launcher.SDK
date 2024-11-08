namespace Launcher;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using WinRT;

static class Versions
{
    static readonly PackageManager PackageManager = new();

    const string Url = "https://raw.githubusercontent.com/flarialmc/newcdn/main/launcher/versions.json";

    internal static async Task<List<(string Name, string Url)>> GetAsync()
    {
        Logger.Log("Attempting to fetch compatible versions.");

        List<(string Name, string Url)> list = [];
        var node = (await JsonNode.ParseAsync(await Internet.StreamAsync(Url)))["versions"];

        foreach (var item in node.AsArray())
        {
            (string Name, string Url) value = new() { Name = item["name"].AsValue().GetValue<string>(), Url = item["verlink"].AsValue().GetValue<string>() };
            Logger.Log($"Found: {value}");
            list.Add(value);
        }

        Logger.Log("Successfully found compatible versions.");
        return list;
    }

    internal static async Task InstallAsync(this (string Name, string Url) source, Action<int> action)
    {
        Logger.Log($"Attempting to install: {source}");
        Uri url = new((await JsonNode.ParseAsync(await Internet.StreamAsync(source.Url)))["url"].AsValue().GetValue<string>());
        await Internet.DownloadAsync(url.AbsoluteUri, Path.GetTempFileName(), action);
        Logger.Log($"Successfully installed: {source}");
    }
}