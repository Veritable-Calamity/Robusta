using System.Text.Json;
using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class MilestoneOneContractTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();
    private static readonly HashSet<string> MilestoneOneProductAdrs =
    [
        "0003", "0004", "0005", "0006", "0007", "0008", "0009", "0011", "0012", "0013"
    ];

    [Fact]
    public void BehavioralCatalogCoversEveryM1ScenarioNamedByTheEvidenceLedger()
    {
        using var ledger = ReadJson("docs/status/evidence/ledger.json");
        using var catalog = ReadJson("docs/specifications/m1-behavioral-scenarios.json");

        var required = ledger.RootElement.GetProperty("entries").EnumerateArray()
            .Where(entry => MilestoneOneProductAdrs.Contains(entry.GetProperty("adr").GetString()!))
            .SelectMany(entry => entry.GetProperty("scenarios").EnumerateArray()
                .Select(scenario => (Adr: entry.GetProperty("adr").GetString()!, Id: scenario.GetString()!)))
            .ToArray();
        var specified = catalog.RootElement.GetProperty("scenarios").EnumerateArray()
            .ToDictionary(
                scenario => scenario.GetProperty("id").GetString()!,
                scenario => scenario.GetProperty("serves").EnumerateArray()
                    .Select(adr => adr.GetString()!).ToHashSet());

        var missing = required
            .Where(item => !specified.TryGetValue(item.Id, out var served) || !served.Contains(item.Adr))
            .Select(item => $"{item.Adr}:{item.Id}")
            .ToArray();

        Assert.Empty(missing);
    }

    [Fact]
    public void BehavioralScenariosHaveUniqueStableTestContracts()
    {
        using var catalog = ReadJson("docs/specifications/m1-behavioral-scenarios.json");
        var root = catalog.RootElement;
        var scenarios = root.GetProperty("scenarios").EnumerateArray().ToArray();
        var gates = root.GetProperty("decisionGates").EnumerateArray()
            .Select(gate => gate.GetProperty("id").GetString()!).ToHashSet();

        Assert.Equal("none", root.GetProperty("implementationClaim").GetString());
        Assert.Equal(scenarios.Length, scenarios.Select(ScenarioId).Distinct().Count());

        var testNames = scenarios.SelectMany(scenario => Strings(scenario, "testNames")).ToArray();
        Assert.Equal(testNames.Length, testNames.Distinct().Count());

        foreach (var scenario in scenarios)
        {
            Assert.Equal("specified", scenario.GetProperty("state").GetString());
            Assert.NotEmpty(Strings(scenario, "serves"));
            Assert.NotEmpty(Strings(scenario, "actors"));
            Assert.NotEmpty(Strings(scenario, "given"));
            Assert.NotEmpty(Strings(scenario, "when"));
            Assert.NotEmpty(Strings(scenario, "then"));
            Assert.NotEmpty(Strings(scenario, "qualityFacets"));
            Assert.All(Strings(scenario, "openDecisionGates"), gate => Assert.Contains(gate, gates));
        }
    }

    [Fact]
    public void BehavioralScenarioSourcesAreAcceptedProductAdrs()
    {
        using var catalog = ReadJson("docs/specifications/m1-behavioral-scenarios.json");
        var accepted = Directory.EnumerateFiles(Path("docs/decisions/product"), "*.md")
            .Where(file => File.ReadAllText(file).Contains("**Decision status:** Accepted", StringComparison.Ordinal))
            .ToDictionary(file => System.IO.Path.GetFileName(file)[..4]);

        foreach (var scenario in catalog.RootElement.GetProperty("scenarios").EnumerateArray())
        {
            Assert.All(Strings(scenario, "serves"), adr => Assert.Contains(adr, accepted.Keys));
            Assert.All(Strings(scenario, "sourceProofs"), source =>
            {
                var file = source.Split('#', 2)[0];
                Assert.True(File.Exists(Path(file)), $"Missing scenario source: {source}");
                Assert.Contains(accepted.Values, acceptedFile =>
                    string.Equals(acceptedFile, Path(file), StringComparison.OrdinalIgnoreCase));
            });
        }
    }

    private static string ScenarioId(JsonElement scenario) => scenario.GetProperty("id").GetString()!;
    private static string[] Strings(JsonElement element, string property) => element.GetProperty(property)
        .EnumerateArray().Select(item => item.GetString()!).ToArray();
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
