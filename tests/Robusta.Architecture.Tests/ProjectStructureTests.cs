using System.Xml.Linq;
using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class ProjectStructureTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void ProjectFileNamesMatchTheirPhysicalDirectories()
    {
        var mismatches = ProjectFiles()
            .Where(path => Path.GetFileNameWithoutExtension(path) != Directory.GetParent(path)!.Name)
            .Select(path => Path.GetRelativePath(RepositoryRoot, path))
            .ToArray();

        Assert.Empty(mismatches);
    }

    [Fact]
    public void GameSdkProjectsDoNotReferenceInternalRuntimeProjects()
    {
        var runtimeRoot = Path.Combine(RepositoryRoot, "src") + Path.DirectorySeparatorChar;
        var invalidReferences = ProjectFiles(Path.Combine(RepositoryRoot, "sdk"))
            .SelectMany(project => References(project)
                .Select(reference => (Project: project, Reference: ResolveReference(project, reference))))
            .Where(pair => pair.Reference.StartsWith(runtimeRoot, StringComparison.OrdinalIgnoreCase))
            .Select(pair => $"{Path.GetRelativePath(RepositoryRoot, pair.Project)} -> {pair.Reference}")
            .ToArray();

        Assert.Empty(invalidReferences);
    }

    [Fact]
    public void ClientAndServerSdkProjectsRemainSeparate()
    {
        var client = Path.Combine(RepositoryRoot, "sdk", "Robusta.Game.Client", "Robusta.Game.Client.csproj");
        var server = Path.Combine(RepositoryRoot, "sdk", "Robusta.Game.Server", "Robusta.Game.Server.csproj");

        Assert.DoesNotContain(References(client), reference => ResolveReference(client, reference) == server);
        Assert.DoesNotContain(References(server), reference => ResolveReference(server, reference) == client);
    }

    private static IEnumerable<string> ProjectFiles(string? root = null) =>
        Directory.EnumerateFiles(root ?? RepositoryRoot, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.Split(Path.DirectorySeparatorChar)
                .Any(segment => segment is "bin" or "obj"));

    private static IEnumerable<string> References(string project) =>
        XDocument.Load(project)
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .OfType<string>();

    private static string ResolveReference(string project, string reference) =>
        Path.GetFullPath(reference, Path.GetDirectoryName(project)!);

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Robusta.slnx")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Could not find the Robusta repository root.");
    }
}
