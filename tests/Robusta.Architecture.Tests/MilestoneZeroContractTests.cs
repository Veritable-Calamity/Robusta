using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class MilestoneZeroContractTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void EveryAcceptedProductAdrHasOneEvidenceLedgerEntry()
    {
        var acceptedAdrs = Directory.EnumerateFiles(Path("docs/decisions/product"), "*.md")
            .Where(file => File.ReadAllText(file).Contains("**Decision status:** Accepted", StringComparison.Ordinal))
            .Select(file => Regex.Match(System.IO.Path.GetFileName(file), "^[0-9]{4}").Value)
            .Order().ToArray();
        using var ledger = ReadJson("docs/status/evidence/ledger.json");
        var ledgerAdrs = ledger.RootElement.GetProperty("entries").EnumerateArray()
            .Select(entry => entry.GetProperty("adr").GetString()).Order().ToArray();

        Assert.Equal(acceptedAdrs, ledgerAdrs);
        Assert.Equal(ledgerAdrs.Length, ledgerAdrs.Distinct().Count());
    }

    [Fact]
    public void LedgerEvidenceLocationsExist()
    {
        using var ledger = ReadJson("docs/status/evidence/ledger.json");
        var missing = ledger.RootElement.GetProperty("entries").EnumerateArray()
            .SelectMany(entry => entry.GetProperty("evidence").EnumerateArray())
            .Select(item => item.GetString()!)
            .Where(location => !File.Exists(Path(location)) && !Directory.Exists(Path(location)))
            .ToArray();
        Assert.Empty(missing);
    }

    [Fact]
    public void CapabilityLabelsAndEvidenceAreVisibleAndValid()
    {
        using var capabilities = ReadJson("docs/status/capabilities.json");
        var allowedLabels = new[] { "Experimental", "Preview", "Supported", "Deprecated", "Removed" };
        var entries = capabilities.RootElement.GetProperty("capabilities").EnumerateArray().ToArray();

        Assert.NotEmpty(entries);
        foreach (var capability in entries)
        {
            Assert.Contains(capability.GetProperty("label").GetString(), allowedLabels);
            Assert.False(string.IsNullOrWhiteSpace(capability.GetProperty("owner").GetString()));
            Assert.NotEmpty(capability.GetProperty("evidence").EnumerateArray());
            foreach (var evidence in capability.GetProperty("evidence").EnumerateArray())
                Assert.True(File.Exists(Path(evidence.GetString()!)), $"Missing capability evidence: {evidence}");
        }
    }

    [Fact]
    public void MigrationCensusAndCorpusCoverTheAcceptedScenarios()
    {
        using var census = ReadJson("docs/status/migration/census-v1.json");
        using var corpus = ReadJson("docs/status/migration/conformance-corpus-v1.json");
        var categories = census.RootElement.GetProperty("categories").EnumerateArray()
            .Select(item => item.GetProperty("id").GetString()).ToHashSet();
        var cases = corpus.RootElement.GetProperty("cases").EnumerateArray().ToArray();

        Assert.Equal(12, cases.Length);
        Assert.All(cases, item => Assert.Contains(item.GetProperty("category").GetString(), categories));
        Assert.All(cases, item => Assert.NotEmpty(item.GetProperty("observations").EnumerateArray()));
    }

    [Fact]
    public void CleanMachineWorkflowCoversTheAcceptedSupportMatrix()
    {
        var workflow = File.ReadAllText(Path(".github/workflows/ci.yml"));
        Assert.Contains("windows-2025", workflow, StringComparison.Ordinal);
        Assert.Contains("ubuntu-24.04", workflow, StringComparison.Ordinal);
        Assert.Contains("verify-external-consumption.ps1", workflow, StringComparison.Ordinal);
    }

    [Fact]
    public void AllMilestoneZeroJsonArtifactsAreParseable()
    {
        var files = Directory.EnumerateFiles(Path("docs/status"), "*.json", SearchOption.AllDirectories).ToArray();
        Assert.NotEmpty(files);
        foreach (var file in files)
        {
            using var document = JsonDocument.Parse(File.ReadAllText(file));
            Assert.Equal(JsonValueKind.Object, document.RootElement.ValueKind);
            var version = document.RootElement.TryGetProperty("schemaVersion", out var instanceVersion)
                ? instanceVersion.GetInt32()
                : document.RootElement.GetProperty("properties").GetProperty("schemaVersion").GetProperty("const").GetInt32();
            Assert.Equal(1, version);
        }
    }

    private static JsonDocument ReadJson(string relativePath) => JsonDocument.Parse(File.ReadAllText(Path(relativePath)));
    private static string Path(string relativePath) => System.IO.Path.Combine(RepositoryRoot, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(System.IO.Path.Combine(directory.FullName, "Robusta.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Could not find repository root.");
    }
}
