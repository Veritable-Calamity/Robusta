using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class SupportedPlatformPipelineTests
{
    [Fact]
    public void CleanMachinePipelineCoversTheAcceptedSupportMatrix()
    {
        var workflow = File.ReadAllText(RepositoryArtifacts.Path(".github/workflows/ci.yml"));

        Assert.Contains("windows-2025", workflow, StringComparison.Ordinal);
        Assert.Contains("ubuntu-24.04", workflow, StringComparison.Ordinal);
        Assert.Contains("verify-external-consumption.ps1", workflow, StringComparison.Ordinal);

        var externalConsumptionVerification = File.ReadAllText(
            RepositoryArtifacts.Path("eng/verify-external-consumption.ps1"));
        Assert.Contains("JsonSchemaValidator", externalConsumptionVerification, StringComparison.Ordinal);
        Assert.Contains("evidence-packet.schema.json", externalConsumptionVerification, StringComparison.Ordinal);
    }
}
