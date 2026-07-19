using Xunit;

namespace Robusta.Architecture.Tests;

public sealed class MigrationBaselineContractTests
{
    [Fact]
    public void EveryMigrationCategoryHasExactlyOneDistinctConformanceCase()
    {
        using var census = RepositoryArtifacts.ReadJson("docs/status/migration/census-v1.json");
        using var corpus = RepositoryArtifacts.ReadJson("docs/status/migration/conformance-corpus-v1.json");
        var categoryIds = census.RootElement.GetProperty("categories").EnumerateArray()
            .Select(item => item.GetProperty("id").GetString()!)
            .ToArray();
        var cases = corpus.RootElement.GetProperty("cases").EnumerateArray().ToArray();
        var caseIds = cases
            .Select(item => item.GetProperty("id").GetString()!)
            .ToArray();
        var caseCategories = cases
            .Select(item => item.GetProperty("category").GetString()!)
            .ToArray();

        Assert.NotEmpty(categoryIds);
        Assert.All(categoryIds, id => Assert.False(string.IsNullOrWhiteSpace(id)));
        Assert.Equal(categoryIds.Length, categoryIds.Distinct().Count());

        Assert.All(caseIds, id => Assert.False(string.IsNullOrWhiteSpace(id)));
        Assert.Equal(caseIds.Length, caseIds.Distinct().Count());
        Assert.Equal(categoryIds.Order(), caseCategories.Order());
        Assert.All(cases, item => Assert.NotEmpty(item.GetProperty("observations").EnumerateArray()));
    }
}
