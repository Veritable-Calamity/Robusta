[CmdletBinding()]
param(
    [string] $Feed = "artifacts/feed",
    [string] $Version = "0.1.0-preview.0"
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$feedPath = [IO.Path]::GetFullPath((Join-Path $repositoryRoot $Feed))
$workPath = Join-Path ([IO.Path]::GetTempPath()) ("robusta-consumer-" + [Guid]::NewGuid().ToString("N"))
$evidencePath = Join-Path $repositoryRoot "artifacts/evidence"

try {
    New-Item -ItemType Directory -Path $workPath | Out-Null
    dotnet new classlib --name ExternalGame --output $workPath --framework net10.0 --no-restore | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Creating the external consumer failed." }

    $nugetConfig = Join-Path $workPath "NuGet.Config"
    @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="robusta-local-artifact-feed" value="$feedPath" />
  </packageSources>
</configuration>
"@ | Set-Content $nugetConfig -Encoding utf8

    dotnet add (Join-Path $workPath "ExternalGame.csproj") package Robusta.Game.Client --version $Version --source $feedPath --no-restore | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Adding the client package failed." }
    dotnet add (Join-Path $workPath "ExternalGame.csproj") package Robusta.Game.Server --version $Version --source $feedPath --no-restore | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "Adding the server package failed." }
    dotnet restore (Join-Path $workPath "ExternalGame.csproj") --configfile $nugetConfig
    if ($LASTEXITCODE -ne 0) { throw "Restoring from the artifact feed failed." }
    dotnet build (Join-Path $workPath "ExternalGame.csproj") --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Building the external consumer failed." }

    $projectText = Get-Content (Join-Path $workPath "ExternalGame.csproj") -Raw
    if ($projectText.IndexOf("ProjectReference", [StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "External-consumption evidence contains a project reference."
    }

    New-Item -ItemType Directory -Force -Path $evidencePath | Out-Null
    $feedIndex = Join-Path $feedPath "index.json"
    $observedAtUtc = [DateTimeOffset]::UtcNow
    $packetId = [String]::Format(
        "external-sdk-package-consumption-{0}-{1}",
        $observedAtUtc.ToString("yyyyMMddTHHmmssfffZ"),
        [Guid]::NewGuid().ToString("N"))
    $packetPath = Join-Path $evidencePath "external-sdk-package-consumption.json"
    [ordered]@{
        schemaVersion = 1
        packetId = $packetId
        scenarioId = "external-sdk-package-consumption"
        capabilityId = "delivery.sdk-package-feed"
        facet = "packaging"
        result = "passed"
        observedAtUtc = $observedAtUtc.ToString("O")
        environment = [ordered]@{
            os = [Environment]::OSVersion.VersionString
            architecture = [Runtime.InteropServices.RuntimeInformation]::OSArchitecture.ToString()
            dotnetSdk = (dotnet --version)
            ciRun = $env:GITHUB_RUN_ID
        }
        subject = [ordered]@{
            repository = "external-temporary-consumer"
            revision = "generated"
            artifactVersions = [ordered]@{
                "Robusta.Game.Client" = $Version
                "Robusta.Game.Server" = $Version
            }
        }
        metrics = @([ordered]@{ name = "project-references"; value = 0; unit = "count"; budget = 0 })
        artifacts = @([ordered]@{
            kind = "artifact-feed-index"
            uri = "artifacts/feed/index.json"
            sha256 = (Get-FileHash $feedIndex -Algorithm SHA256).Hash.ToLowerInvariant()
        })
        notes = "A temporary project outside the repository restored and built using only feed packages."
    } | ConvertTo-Json -Depth 6 | Set-Content $packetPath -Encoding utf8

    $schemaValidatorProject = Join-Path $repositoryRoot "tools/JsonSchemaValidator/JsonSchemaValidator.csproj"
    $evidencePacketSchema = Join-Path $repositoryRoot "docs/status/evidence/evidence-packet.schema.json"
    $validatorArguments = @(
        "run",
        "--project", $schemaValidatorProject,
        "--configuration", "Release",
        "--no-build",
        "--no-restore",
        "--",
        $evidencePacketSchema,
        $packetPath
    )
    dotnet @validatorArguments
    if ($LASTEXITCODE -ne 0) { throw "The external-consumption evidence packet failed schema validation." }
}
finally {
    if (Test-Path -LiteralPath $workPath) {
        Remove-Item -LiteralPath $workPath -Recurse -Force
    }
}
