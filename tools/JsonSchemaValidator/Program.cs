using System.Text.Json;
using Json.Schema;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: Robusta.Tools.JsonSchemaValidator <schema-path> <document-path>");
    return 2;
}

var schemaPath = Path.GetFullPath(args[0]);
var documentPath = Path.GetFullPath(args[1]);

try
{
    var buildOptions = new BuildOptions
    {
        Dialect = Dialect.Draft202012,
        SchemaRegistry = new SchemaRegistry()
    };
    var schema = JsonSchema.FromText(
        File.ReadAllText(schemaPath),
        buildOptions,
        new Uri(schemaPath));

    using var document = JsonDocument.Parse(File.ReadAllText(documentPath));
    var results = schema.Evaluate(
        document.RootElement,
        new EvaluationOptions
        {
            OutputFormat = OutputFormat.List,
            RequireFormatValidation = true
        });

    if (!results.IsValid)
    {
        Console.Error.WriteLine($"'{documentPath}' does not satisfy '{schemaPath}'.");
        Console.Error.WriteLine(JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
        return 1;
    }

    Console.WriteLine($"Validated '{documentPath}' against '{schemaPath}'.");
    return 0;
}
catch (Exception exception) when (exception is IOException or JsonException or JsonSchemaException)
{
    Console.Error.WriteLine(exception.Message);
    return 2;
}
