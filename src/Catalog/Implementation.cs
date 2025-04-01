using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Net.Http;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Management.Deployment;
using System.IO.Compression;
using Bedrockix.Minecraft;
using System.Threading;

namespace Flarial.Launcher.SDK;

public sealed partial class Catalog : IEnumerable<string>
{
    static readonly PackageManager Manager = new();

    static readonly SemaphoreSlim Semaphore = new(1, 1);

    readonly Dictionary<string, string> Collection;

    static readonly string Content = new Func<string>(() =>
    {
        using StreamReader reader = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("GetExtendedUpdateInfo2.xml"));
        return reader.ReadToEnd();
    })();

    static readonly AddPackageOptions Options = new()
    {
        ForceAppShutdown = true,
        ForceUpdateFromAnyVersion = true
    };

    Catalog(Dictionary<string, string> value) => Collection = value;

    static string Get(string value)
    {
        var substrings = value.Split('.');
        ushort major = ushort.Parse(substrings[0]), minor, build;

        if (major is 0)
        {
            minor = ushort.Parse(substrings[1].Substring(0, 2));
            build = ushort.Parse(substrings[1].Substring(2));
        }
        else
        {
            minor = ushort.Parse(substrings[1]);
            build = (ushort)(ushort.Parse(substrings[2]) / 100);
        }

        return $"{major}.{minor}.{build}";
    }

    public static async partial Task<Catalog> GetAsync() => await Task.Run(async () =>
    {
        Dictionary<string, string> value = [];
        var collection = await Web.SupportedAsync();

        foreach (var item in await Web.VersionsAsync())
        {
            var substrings = item.Split(' ');

            var identity = substrings[1].Split('_'); if (identity[2] is not "x64") continue;

            var key = Get(identity[1]);
            if (!collection.Contains(key)) continue;

            if (!value.ContainsKey(key)) value.Add(key, substrings[0]);
            else value[key] = substrings[0];
        }

        return new Catalog(value);
    });

    public static partial async Task FrameworksAsync()
    {
        await Semaphore.WaitAsync();
        try
        {
            if (Manager.FindPackagesForUser(string.Empty, "Microsoft.Services.Store.Engagement_8wekyb3d8bbwe").Any()) return;

            await Task.Run(async () =>
            {
                var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                using (var stream = await Web.FrameworkAsync()) using (ZipArchive archive = new(stream))
                    archive.Entries.First(_ => _.Name is "Microsoft.Services.Store.Engagement.x64.10.0.appx").ExtractToFile(path, true);

                await Manager.AddPackageAsync(new(path), default, default);
            });
        }
        finally { Semaphore.Release(); }
    }

    async Task<Uri> PackageAsync(string value) => await Task.Run(async () =>
    {
        using StringContent content = new(string.Format(Content, Collection[value], '1'), Encoding.UTF8, "application/soap+xml");
        await FrameworksAsync(); return await Web.UriAsync(content);
    });

    public partial async Task<Uri> UriAsync(string value) => await PackageAsync(value);

    public async partial Task<bool> CompatibleAsync() => await Task.Run(() => Collection.ContainsKey(Metadata.Version));

    public async partial Task<Request> InstallAsync(string value, Action<int> action) => new(Manager.AddPackageByUriAsync(await PackageAsync(value), Options), action);

    /// <summary>
    /// Enumerates versions present in the catalog.
    /// </summary>

    public IEnumerator<string> GetEnumerator() => Collection.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}