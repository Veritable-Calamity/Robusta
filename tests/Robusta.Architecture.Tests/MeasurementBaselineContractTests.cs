using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class MeasurementBaselineContractTests
{
    [Fact]
    public void CollectionPlansUseDescriptiveOutcomesAndSeparateRoadmapMetadata()
    {
        using var baseline = RepositoryArtifacts.ReadJson("docs/status/metrics-baseline.json");
        var metrics = baseline.RootElement.GetProperty("metrics").EnumerateArray().ToArray();
        var metricIds = metrics.Select(metric => metric.GetProperty("id").GetString()).ToArray();

        Assert.NotEmpty(metrics);
        Assert.Equal(metricIds.Length, metricIds.Distinct().Count());

        foreach (var metric in metrics)
        {
            Assert.Matches(
                "^[a-z][a-z0-9-]+$",
                metric.GetProperty("firstCollectionOutcome").GetString()!);
            Assert.Matches("^M[0-9]+$", metric.GetProperty("plannedMilestone").GetString()!);
            Assert.False(metric.TryGetProperty("firstCollection", out _));
        }
    }
}
