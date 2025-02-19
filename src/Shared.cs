using System.Net.Http;

namespace Flarial.Launcher;

static class Shared
{
    internal static readonly HttpClient HttpClient = new();
}