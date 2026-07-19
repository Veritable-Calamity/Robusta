using System.Text.RegularExpressions;
using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class EvidenceLedgerContractTests
{
    [Fact]
    public void EveryAcceptedProductAdrHasOneEvidenceLedgerEntry()
    {
        var acceptedAdrs = Directory
            .EnumerateFiles(RepositoryArtifacts.Path("docs/decisions/product"), "*.md")
            .Where(file => File.ReadAllText(file).Contains("**Decision status:** Accepted", StringComparison.Ordinal))
            .Select(file => Regex.Match(System.IO.Path.GetFileName(file), "^[0-9]{4}").Value)
            .Order()
            .ToArray();
        using var ledger = RepositoryArtifacts.ReadJson("docs/status/evidence/ledger.json");
        var ledgerAdrs = ledger.RootElement.GetProperty("entries").EnumerateArray()
            .Select(entry => entry.GetProperty("adr").GetString())
            .Order()
            .ToArray();

        Assert.Equal(acceptedAdrs, ledgerAdrs);
        Assert.Equal(ledgerAdrs.Length, ledgerAdrs.Distinct().Count());
    }

    [Fact]
    public void EveryLedgerEvidenceLocationExists()
    {
        using var ledger = RepositoryArtifacts.ReadJson("docs/status/evidence/ledger.json");
        var missing = ledger.RootElement.GetProperty("entries").EnumerateArray()
            .SelectMany(entry => entry.GetProperty("evidence").EnumerateArray())
            .Select(item => item.GetString()!)
            .Where(location =>
                !File.Exists(RepositoryArtifacts.Path(location)) &&
                !Directory.Exists(RepositoryArtifacts.Path(location)))
            .ToArray();

        Assert.Empty(missing);
    }

    [Fact]
    public void LedgerTargetsUseDescriptiveOutcomesAndSeparateRoadmapMetadata()
    {
        using var ledger = RepositoryArtifacts.ReadJson("docs/status/evidence/ledger.json");

        foreach (var entry in ledger.RootElement.GetProperty("entries").EnumerateArray())
        {
            var targetOutcome = entry.GetProperty("targetOutcome").GetString();
            var plannedMilestone = entry.GetProperty("plannedMilestone").GetString();

            Assert.Matches("^[a-z][a-z0-9-]+$", targetOutcome!);
            Assert.Matches("^M[0-9]+$", plannedMilestone!);
            Assert.False(entry.TryGetProperty("targetMilestone", out _));
        }
    }
}
