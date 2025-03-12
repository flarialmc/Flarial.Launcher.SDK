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
    const string Supported = "https://raw.githubusercontent.com/flarialmc/newcdn/main/launcher/NewSupported.txt";

    const string Store = "https://fe3cr.delivery.mp.microsoft.com/ClientWebService/client.asmx/secured";

    readonly static Version Version = new("1.21.51");

    static readonly PackageManager PackageManager = new();

    static string _(string value)
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
        var set = (await Web.GetAsync(Supported)).Split('\n').ToHashSet();

        foreach (var item in await Web.PackagesAsync())
        {
            var substrings = item.Split(' ');

            var identity = substrings[1].Split('_'); if (identity[2] is not "x64") continue;

            var key = _(identity[1]);
            if (!set.Contains(key)) continue;

            if (!value.ContainsKey(key)) value.Add(key, substrings[0]);
            else value[key] = substrings[0];
        }

        return new Catalog(value);
    });

    /// <summary>
    /// Enumerates versions present in the catalog.
    /// </summary>

    public IEnumerator<string> GetEnumerator() => Value.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    readonly Dictionary<string, string> Value;

    static string Content;

    Catalog(Dictionary<string, string> value) => Value = value;

    static async Task<string> GetExtendedUpdateInfo2()
    {
        if (Content is null)
        {
            using StreamReader reader = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("GetExtendedUpdateInfo2.xml"));
            Content = await reader.ReadToEndAsync();
        }
        return Content;
    }

    static async Task<Uri> UriAsync(string value)
    {
        using StringContent content = new(string.Format(await GetExtendedUpdateInfo2(), value, '1'), Encoding.UTF8, "application/soap+xml");
        using var message = await Web.PostAsync(Store, content); message.EnsureSuccessStatusCode();
        return new(await Task.Run(async () =>
        {
            return XElement.Parse(await message.Content.ReadAsStringAsync()).Descendants().
            FirstOrDefault(_ => _.Value.StartsWith("http://tlu.dl.delivery.mp.microsoft.com", StringComparison.Ordinal)).Value;
        }));
    }

    static readonly AddPackageOptions Options = new()
    {
        ForceAppShutdown = true,
        ForceUpdateFromAnyVersion = true
    };

    public async partial Task<bool> CompatibleAsync() => await Task.Run(() => Value.ContainsKey(Metadata.Version));

    public async partial Task<Request> InstallAsync(string value, Action<int> action) => await Task.Run(async () =>
    {
        var _ = Value[value];

        if (new Version(value) <= Version && !PackageManager.FindPackagesForUser(string.Empty, "Microsoft.Services.Store.Engagement_8wekyb3d8bbwe").Any())
        {
            var path = Path.Combine(Path.GetTempPath(), "Microsoft.Services.Store.Engagement.x64.10.0.appx");

            using var stream = await Web.FrameworkAsync(); using ZipArchive archive = new(stream);
            archive.Entries.First(_ => _.Name is "Microsoft.Services.Store.Engagement.x64.10.0.appx").ExtractToFile(path, true);

            var @object = PackageManager.AddPackageAsync(new Uri(path), default, default);

            if (@object.Status is AsyncStatus.Started)
            {
                using ManualResetEventSlim @event = new();
                @object.Completed += (_, _) => @event.Set();
                @event.Wait();
            }

            if (@object.Status is AsyncStatus.Error) throw @object.ErrorCode;
        }

        return new Request(PackageManager.AddPackageByUriAsync(await UriAsync(_), Options), action);
    });
}