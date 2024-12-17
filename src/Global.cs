using System.Net.Http;
using Windows.Management.Deployment;

static class Global
{
    internal static readonly HttpClient HttpClient = new();

    internal const string PackageFamilyName = "Microsoft.MinecraftUWP_8wekyb3d8bbwe";
}