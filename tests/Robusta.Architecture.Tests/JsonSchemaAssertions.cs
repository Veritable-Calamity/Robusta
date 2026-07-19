using System.Text.Json;
using Json.Schema;
using Xunit;

namespace Robusta.Architecture.Tests;

internal static class JsonSchemaAssertions
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public static void Validates(string schemaRelativePath, string documentRelativePath)
    {
        var schemaPath = RepositoryArtifacts.Path(schemaRelativePath);
        var documentPath = RepositoryArtifacts.Path(documentRelativePath);
        var schema = JsonSchema.FromText(
            File.ReadAllText(schemaPath),
            new BuildOptions
            {
                Dialect = Dialect.Draft202012,
                SchemaRegistry = new SchemaRegistry()
            },
            new Uri(schemaPath));

        using var document = JsonDocument.Parse(File.ReadAllText(documentPath));
        var results = schema.Evaluate(
            document.RootElement,
            new EvaluationOptions
            {
                OutputFormat = OutputFormat.List,
                RequireFormatValidation = true
            });

        Assert.True(
            results.IsValid,
            $"{documentRelativePath} does not satisfy {schemaRelativePath}:{Environment.NewLine}" +
            JsonSerializer.Serialize(results, SerializerOptions));
    }
}
