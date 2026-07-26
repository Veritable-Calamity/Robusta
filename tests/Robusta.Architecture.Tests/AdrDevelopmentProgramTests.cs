using System.Text.RegularExpressions;
using Xunit;

namespace Robusta.Architecture.Tests;

public sealed partial class AdrDevelopmentProgramTests
{
    [Fact]
    public void EveryRoadmapQuestionMapsExactlyOnceToOneUniqueProgramPackage()
    {
        var roadmap = File.ReadAllText(
            RepositoryArtifacts.Path("docs/status/platform-development-roadmap.md"));
        var program = File.ReadAllText(
            RepositoryArtifacts.Path("docs/status/adr-development-program.md"));

        var sourceInventory = Section(
            roadmap,
            "## Source ADR question inventory",
            "## Parallel work lanes");
        var ledger = Section(
            program,
            "## Program ledger",
            "## Dependency waves");

        var sourceIds = SourceRow()
            .Matches(sourceInventory)
            .Select(match => match.Groups["id"].Value)
            .ToArray();
        var packageIds = PackageRow()
            .Matches(ledger)
            .Select(match => match.Groups["id"].Value)
            .ToArray();
        var specificationIds = SpecificationRow()
            .Matches(ledger)
            .Select(match => match.Groups["id"].Value)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(99, sourceIds.Length);
        Assert.Equal(sourceIds.Length, sourceIds.Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(70, packageIds.Length);
        Assert.Equal(packageIds.Length, packageIds.Distinct(StringComparer.Ordinal).Count());

        var invalidMappings = sourceIds
            .Select(id => (Id: id, Count: Token(id).Count(ledger)))
            .Where(mapping => mapping.Count != 1)
            .Select(mapping => $"{mapping.Id} occurs {mapping.Count} times")
            .ToArray();

        Assert.Empty(invalidMappings);
        Assert.Equal(7, packageIds.Count(id => id.StartsWith("PRD-", StringComparison.Ordinal)));
        Assert.Equal(5, specificationIds.Count);
        Assert.Equal(
            58,
            packageIds.Count(id =>
                !id.StartsWith("PRD-", StringComparison.Ordinal)
                && !specificationIds.Contains(id)));
    }

    private static string Section(string text, string startHeading, string endHeading)
    {
        var start = text.IndexOf(startHeading, StringComparison.Ordinal);
        var end = text.IndexOf(endHeading, start + startHeading.Length, StringComparison.Ordinal);

        if (start < 0 || end < 0)
            throw new InvalidDataException($"Could not find section {startHeading} through {endHeading}.");

        return text[start..end];
    }

    private static Regex Token(string value) =>
        new($"(?<![A-Z0-9-]){Regex.Escape(value)}(?![A-Z0-9-])", RegexOptions.CultureInvariant);

    [GeneratedRegex(@"^\| (?<id>[A-Z][A-Z0-9-]*-[A-Z0-9-]+) \|", RegexOptions.Multiline)]
    private static partial Regex SourceRow();

    [GeneratedRegex(@"^\| `(?<id>[A-Z][A-Z0-9-]+)` \|", RegexOptions.Multiline)]
    private static partial Regex PackageRow();

    [GeneratedRegex(
        @"^\| `(?<id>[A-Z][A-Z0-9-]+)` \| \*\*Specification first",
        RegexOptions.Multiline)]
    private static partial Regex SpecificationRow();
}
