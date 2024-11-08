using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

static class Internet
{
    static readonly HttpClient Client = new();

    static readonly int Length = Environment.SystemPageSize;

    internal static async Task DownloadAsync(string url, string path, Action<int> action)
    {
        using var message = await Client.GetAsync(url);
        long length = message.Content.Headers.ContentLength.Value;
        using var stream = await message.Content.ReadAsStreamAsync();
        using FileStream destination = new(path, FileMode.Create);

        var value = 0; var count = 0; var buffer = new byte[Length];
        while ((count = await stream.ReadAsync(buffer)) != 0)
        {
            await destination.WriteAsync(buffer);
            action((int)((value += count) * 100 / length));
        }
    }
}