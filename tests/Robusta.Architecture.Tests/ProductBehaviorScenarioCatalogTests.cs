using System.Text.Json;
using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class ProductBehaviorScenarioCatalogTests
{
    private const string CatalogPath = "docs/specifications/product-behavior-scenarios.json";
    private const string SchemaPath = "docs/specifications/behavioral-scenarios.schema.json";

    [Fact]
    public void CatalogSatisfiesItsDeclaredJsonSchema() =>
        JsonSchemaAssertions.Validates(SchemaPath, CatalogPath);

    [Fact]
    public void CatalogMatchesEveryScenarioPairInItsDeclaredDecisionScope()
    {
        using var ledger = RepositoryArtifacts.ReadJson("docs/status/evidence/ledger.json");
        using var catalog = RepositoryArtifacts.ReadJson(CatalogPath);

        var scope = Strings(catalog.RootElement, "sourceDecisionIds").ToHashSet();
        var required = ledger.RootElement.GetProperty("entries").EnumerateArray()
            .Where(entry => scope.Contains(entry.GetProperty("adr").GetString()!))
            .SelectMany(entry => Strings(entry, "scenarios")
                .Select(scenarioId => (DecisionId: entry.GetProperty("adr").GetString()!, ScenarioId: scenarioId)))
            .ToHashSet();
        var specified = catalog.RootElement.GetProperty("scenarios").EnumerateArray()
            .SelectMany(scenario => Strings(scenario, "sourceDecisionIds")
                .Select(decisionId => (DecisionId: decisionId, ScenarioId: ScenarioId(scenario))))
            .ToArray();
        var specifiedSet = specified.ToHashSet();

        Assert.Equal(specified.Length, specifiedSet.Count);
        Assert.Empty(required.Except(specifiedSet));
        Assert.Empty(specifiedSet.Except(required));
        Assert.True(scope.SetEquals(specified.Select(pair => pair.DecisionId)));
    }

    [Fact]
    public void ScenariosHaveUniqueStableContractsAndKnownDependencies()
    {
        using var catalog = RepositoryArtifacts.ReadJson(CatalogPath);
        using var schema = RepositoryArtifacts.ReadJson(SchemaPath);
        var root = catalog.RootElement;
        var scenarios = root.GetProperty("scenarios").EnumerateArray().ToArray();
        var sourceDecisionIds = Strings(root, "sourceDecisionIds");
        var gates = root.GetProperty("decisionGates").EnumerateArray()
            .ToDictionary(gate => gate.GetProperty("id").GetString()!);

        Assert.Equal("product-behavior", root.GetProperty("catalogId").GetString());
        Assert.Equal(2, root.GetProperty("schemaVersion").GetInt32());
        Assert.Equal(schema.RootElement.GetProperty("$id").GetString(), root.GetProperty("$schema").GetString());
        Assert.Equal(sourceDecisionIds.Length, sourceDecisionIds.Distinct().Count());
        Assert.Equal(scenarios.Length, scenarios.Select(ScenarioId).Distinct().Count());

        var conformanceTestIds = scenarios.SelectMany(scenario => Strings(scenario, "conformanceTestIds")).ToArray();
        Assert.Equal(conformanceTestIds.Length, conformanceTestIds.Distinct().Count());

        foreach (var scenario in scenarios)
        {
            Assert.Equal("specified", scenario.GetProperty("specificationStatus").GetString());
            Assert.NotEmpty(Strings(scenario, "sourceDecisionIds"));
            Assert.All(Strings(scenario, "sourceDecisionIds"), decisionId => Assert.Contains(decisionId, sourceDecisionIds));
            Assert.NotEmpty(Strings(scenario, "sourceProofRefs"));
            Assert.NotEmpty(Strings(scenario, "actors"));
            Assert.NotEmpty(Strings(scenario, "given"));
            Assert.NotEmpty(Strings(scenario, "when"));
            Assert.NotEmpty(Strings(scenario, "then"));
            Assert.NotEmpty(Strings(scenario, "qualityFacets"));
            Assert.All(Strings(scenario, "decisionDependencies"), dependency => Assert.Contains(dependency, gates.Keys));
        }
    }

    [Fact]
    public void EntityLifecycleAndSimulationTimeAreIndependentDecisionDependencies()
    {
        using var catalog = RepositoryArtifacts.ReadJson(CatalogPath);
        var gates = catalog.RootElement.GetProperty("decisionGates").EnumerateArray()
            .ToDictionary(gate => gate.GetProperty("id").GetString()!);

        Assert.DoesNotContain("product.entity-time", gates.Keys);
        Assert.Equal(
            "docs/decisions/product/0015-give-entities-an-atomic-observable-lifecycle.md",
            gates["product.entity-lifecycle"].GetProperty("sourceRef").GetString());
        Assert.Equal(
            "docs/decisions/product/0016-separate-simulation-host-and-presentation-time.md",
            gates["product.simulation-time"].GetProperty("sourceRef").GetString());
    }

    [Fact]
    public void ScenarioSourcesAreAcceptedProductDecisions()
    {
        using var catalog = RepositoryArtifacts.ReadJson(CatalogPath);
        var accepted = Directory.EnumerateFiles(RepositoryArtifacts.Path("docs/decisions/product"), "*.md")
            .Where(file => File.ReadAllText(file).Contains("**Decision status:** Accepted", StringComparison.Ordinal))
            .ToDictionary(file => System.IO.Path.GetFileName(file)[..4]);

        foreach (var scenario in catalog.RootElement.GetProperty("scenarios").EnumerateArray())
        {
            Assert.All(Strings(scenario, "sourceDecisionIds"), decisionId => Assert.Contains(decisionId, accepted.Keys));
            Assert.All(Strings(scenario, "sourceProofRefs"), sourceRef =>
            {
                var file = sourceRef.Split('#', 2)[0];
                Assert.True(File.Exists(RepositoryArtifacts.Path(file)), $"Missing scenario source: {sourceRef}");
                Assert.Contains(accepted.Values, acceptedFile =>
                    string.Equals(acceptedFile, RepositoryArtifacts.Path(file), StringComparison.OrdinalIgnoreCase));
            });
        }
    }

    [Fact]
    public void DecisionDependenciesReferenceCurrentSourcesWithoutRequiringProposals()
    {
        var register = File.ReadAllText(RepositoryArtifacts.Path("docs/decisions/README.md"));
        using var catalog = RepositoryArtifacts.ReadJson(CatalogPath);
        var gates = catalog.RootElement.GetProperty("decisionGates").EnumerateArray().ToArray();

        Assert.Equal(gates.Length, gates.Select(gate => gate.GetProperty("id").GetString()).Distinct().Count());

        foreach (var gate in gates)
        {
            var id = gate.GetProperty("id").GetString()!;
            var decisionLevel = gate.GetProperty("decisionLevel").GetString()!;
            var decisionStatus = gate.GetProperty("decisionStatus").GetString()!;
            var sourceRef = gate.GetProperty("sourceRef").GetString()!;
            var sourceFile = sourceRef.Split('#', 2)[0];

            Assert.StartsWith($"{decisionLevel}.", id, StringComparison.Ordinal);
            Assert.True(File.Exists(RepositoryArtifacts.Path(sourceFile)), $"Missing decision source: {sourceRef}");

            if (!sourceFile.StartsWith("docs/decisions/", StringComparison.Ordinal))
                continue;

            var decisionId = System.IO.Path.GetFileName(sourceFile)[..4];
            var statusLine = File.ReadLines(RepositoryArtifacts.Path(sourceFile))
                .Single(line => line.StartsWith("- **Decision status:** ", StringComparison.Ordinal));
            var actualStatus = statusLine["- **Decision status:** ".Length..].Trim().ToLowerInvariant();

            Assert.Contains($"| {decisionId} |", register, StringComparison.Ordinal);
            Assert.Equal(decisionStatus, actualStatus);
        }
    }

    private static string ScenarioId(JsonElement scenario) => scenario.GetProperty("id").GetString()!;
    private static string[] Strings(JsonElement element, string property) => element.GetProperty(property)
        .EnumerateArray().Select(item => item.GetString()!).ToArray();
}
