using System.Text.Json;
using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class StatusArtifactSchemaTests
{
    [Fact]
    public void EveryStatusJsonArtifactIsAParseableVersionedObject()
    {
        var files = Directory
            .EnumerateFiles(RepositoryArtifacts.Path("docs/status"), "*.json", SearchOption.AllDirectories)
            .ToArray();

        Assert.NotEmpty(files);
        foreach (var file in files)
        {
            using var document = JsonDocument.Parse(File.ReadAllText(file));
            Assert.Equal(JsonValueKind.Object, document.RootElement.ValueKind);

            var version = DeclaredSchemaVersion(document.RootElement);
            Assert.True(version > 0, $"{file} declares invalid schema version {version}.");
        }
    }

    private static int DeclaredSchemaVersion(JsonElement root) =>
        root.TryGetProperty("schemaVersion", out var instanceVersion)
            ? instanceVersion.GetInt32()
            : root.GetProperty("properties").GetProperty("schemaVersion").GetProperty("const").GetInt32();
}
