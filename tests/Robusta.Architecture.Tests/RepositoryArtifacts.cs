using System.Text.Json;

namespace Robusta.Architecture.Tests;

internal static class RepositoryArtifacts
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    public static JsonDocument ReadJson(string relativePath) =>
        JsonDocument.Parse(File.ReadAllText(Path(relativePath)));

    public static string Path(string relativePath) =>
        System.IO.Path.Combine(
            RepositoryRoot,
            relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(System.IO.Path.Combine(directory.FullName, "Robusta.slnx")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Could not find the Robusta repository root.");
    }
}
