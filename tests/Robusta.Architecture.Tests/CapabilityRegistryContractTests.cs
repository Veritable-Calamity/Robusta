using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class CapabilityRegistryContractTests
{
    [Fact]
    public void CapabilityLabelsOwnersAndEvidenceAreValid()
    {
        using var capabilities = RepositoryArtifacts.ReadJson("docs/status/capabilities.json");
        var allowedLabels = new[] { "Experimental", "Preview", "Supported", "Deprecated", "Removed" };
        var entries = capabilities.RootElement.GetProperty("capabilities").EnumerateArray().ToArray();

        Assert.NotEmpty(entries);
        foreach (var capability in entries)
        {
            Assert.Contains(capability.GetProperty("label").GetString(), allowedLabels);
            Assert.False(string.IsNullOrWhiteSpace(capability.GetProperty("owner").GetString()));
            Assert.NotEmpty(capability.GetProperty("evidence").EnumerateArray());

            foreach (var evidence in capability.GetProperty("evidence").EnumerateArray())
            {
                Assert.True(
                    File.Exists(RepositoryArtifacts.Path(evidence.GetString()!)),
                    $"Missing capability evidence: {evidence}");
            }
        }
    }
}
