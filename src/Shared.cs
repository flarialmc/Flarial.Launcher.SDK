using System.Net.Http;

namespace Flarial.Launcher.SDK;

static class Shared
{
    internal static readonly HttpClient HttpClient = new();
}