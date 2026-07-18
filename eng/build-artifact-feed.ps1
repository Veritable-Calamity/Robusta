[CmdletBinding()]
param(
    [string] $Configuration = "Release",
    [string] $Output = "artifacts/feed",
    [string] $Version = "0.1.0-preview.0"
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$feedPath = Join-Path $repositoryRoot $Output
New-Item -ItemType Directory -Force -Path $feedPath | Out-Null

Get-ChildItem (Join-Path $repositoryRoot "sdk") -Filter *.csproj -Recurse |
    Sort-Object FullName |
    ForEach-Object {
        dotnet pack $_.FullName --configuration $Configuration --output $feedPath --no-restore -p:PackageVersion=$Version
        if ($LASTEXITCODE -ne 0) { throw "Packing $($_.Name) failed." }
    }

$packages = Get-ChildItem $feedPath -Filter "*.nupkg" | Sort-Object Name | ForEach-Object {
    [ordered]@{
        file = $_.Name
        sha256 = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    }
}

$index = [ordered]@{
    schemaVersion = 1
    feedVersion = $Version
    generatedAtUtc = [DateTimeOffset]::UtcNow.ToString("O")
    packages = @($packages)
}
$index | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $feedPath "index.json") -Encoding utf8
