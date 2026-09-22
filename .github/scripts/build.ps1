# Run in a disposable checkout: this regenerates shaders and the parent targets file.
param(
    [Parameter(Mandatory)][string] $TmlPath,
    [Parameter(Mandatory)][string] $SavePath,
    [Parameter(Mandatory)][string] $ArtifactPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$modPath = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$TmlPath = (Resolve-Path -LiteralPath $TmlPath).Path
$SavePath = [IO.Path]::GetFullPath($SavePath)
$ArtifactPath = [IO.Path]::GetFullPath($ArtifactPath)
$targets = Join-Path $TmlPath 'tMLMod.targets'
if (!(Test-Path -LiteralPath $targets)) { throw "Missing $targets" }

New-Item -ItemType Directory -Force $SavePath, $ArtifactPath | Out-Null
$escapedTargets = [Security.SecurityElement]::Escape($targets)
@"
<Project>
  <Import Project="$escapedTargets" />
</Project>
"@ | Set-Content -LiteralPath (Join-Path (Split-Path $modPath) 'tModLoader.targets') -Encoding utf8

Push-Location $modPath
try {
    # Use the same SDK locally and on hosted runners, which have multiple SDKs installed.
    '{"sdk":{"version":"8.0.425","rollForward":"disable"}}' |
        Set-Content -LiteralPath (Join-Path (Split-Path $modPath) 'global.json') -Encoding utf8

    # Always regenerate tracked shader outputs; never trust checkout timestamps.
    # Check each invocation because the project's BuildEffects target ignores errors.
    $compiler = Join-Path $modPath 'Assets/Effects/Compiler/fxc.exe'
    $effects = @(Get-ChildItem -LiteralPath $modPath -Recurse -Filter '*.fx' -File)
    if ($effects.Count -eq 0) { throw 'No shader sources found' }
    foreach ($effect in $effects) {
        $output = [IO.Path]::ChangeExtension($effect.FullName, '.fxc')
        "Compiling $($effect.FullName)" | Tee-Object -FilePath "$ArtifactPath/shaders.log" -Append
        & $compiler $effect.FullName /T fx_2_0 /nologo /O2 /Fo $output 2>&1 |
            Tee-Object -FilePath "$ArtifactPath/shaders.log" -Append
        if ($LASTEXITCODE -ne 0) { throw "Shader compilation failed: $($effect.Name)" }
        if (!(Test-Path -LiteralPath $output)) { throw "Missing compiled shader: $output" }
    }

    dotnet build Macrocosm.csproj --configuration Release --nologo `
        "-p:ExtraBuildModFlags=-tmlsavedirectory `"$SavePath`" -nosteam" `
        "-bl:$ArtifactPath/build.binlog" 2>&1 |
        Tee-Object -FilePath "$ArtifactPath/build.log"
    if ($LASTEXITCODE -ne 0) { throw 'Macrocosm build failed' }

    $package = Join-Path $SavePath 'Mods/Macrocosm.tmod'
    if (!(Test-Path -LiteralPath $package) -or (Get-Item -LiteralPath $package).Length -eq 0) {
        throw 'Build did not produce Macrocosm.tmod'
    }
    Copy-Item -LiteralPath $package -Destination (Join-Path $ArtifactPath 'Macrocosm.tmod')
}
finally {
    Pop-Location
}
