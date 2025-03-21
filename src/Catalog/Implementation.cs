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
using System.Threading;
using Windows.Foundation;
using Bedrockix.Minecraft;

namespace Flarial.Launcher.SDK;

public sealed partial class Catalog : IEnumerable<string>
{
    readonly static Version Version = new("1.21.51");

    static readonly PackageManager Manager = new();

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

    static async Task DependencyAsync(string value) => await Task.Run(async () =>
    {
        if (new Version(value) <= Version && !Manager.FindPackagesForUser(string.Empty, "Microsoft.Services.Store.Engagement_8wekyb3d8bbwe").Any())
        {
            var path = Path.Combine(Path.GetTempPath(), "Microsoft.Services.Store.Engagement.x64.10.0.appx");

            using var stream = await Web.FrameworkAsync(); using ZipArchive archive = new(stream);
            archive.Entries.First(_ => _.Name is "Microsoft.Services.Store.Engagement.x64.10.0.appx").ExtractToFile(path, true);

            var @object = Manager.AddPackageAsync(new Uri(path), default, default);

            if (@object.Status is AsyncStatus.Started)
            {
                using ManualResetEventSlim @event = new();
                @object.Completed += (_, _) => @event.Set();
                @event.Wait();
            }

            if (@object.Status is AsyncStatus.Error) throw @object.ErrorCode;
        }
    });

    static async Task<Uri> GetAsync(string value) => await Task.Run(async () =>
    {
        using StringContent content = new(string.Format(Content, value, '1'), Encoding.UTF8, "application/soap+xml");
        return await Web.UriAsync(content);
    });

    public partial async Task<Uri> UriAsync(string value) => await GetAsync(Collection[value]);

    public async partial Task<bool> CompatibleAsync() => await Task.Run(() => Collection.ContainsKey(Metadata.Version));

    public async partial Task<Request> InstallAsync(string value, Action<int> action)
    {
        var @this = Collection[value];
        await DependencyAsync(value);
        return new Request(Manager.AddPackageByUriAsync(await GetAsync(@this), Options), action);
    }

    /// <summary>
    /// Enumerates versions present in the catalog.
    /// </summary>

    public IEnumerator<string> GetEnumerator() => Collection.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}