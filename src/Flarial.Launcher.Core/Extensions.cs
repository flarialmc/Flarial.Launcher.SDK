using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

static class Extensions
{
    static readonly int Size = Environment.SystemPageSize;

    internal static async Task GetAsync(this HttpClient source, string requestUri, string path, Action<int> action = default)
    {
        using var message = await source.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);

        using var stream = await message.Content.ReadAsStreamAsync();
        using var destination = File.OpenWrite(path);

        var count = 0;
        var value = 0L;
        var buffer = new byte[Size];

        while ((count = await stream.ReadAsync(buffer, 0, buffer.Length)) is not 0)
        {
            await destination.WriteAsync(buffer, 0, count);
            if (action is not null) action((int)Math.Round(100F * (value += count) / message.Content.Headers.ContentLength.Value));
        }
    }
}