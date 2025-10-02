\
Param(
  [string]$Configuration = "Release"
)

# Locate vswhere
$vswhere1 = "$Env:ProgramFiles(x86)\Microsoft Visual Studio\Installer\vswhere.exe"
$vswhere2 = "$Env:ProgramFiles\Microsoft Visual Studio\Installer\vswhere.exe"
$vswhere = $null
if (Test-Path $vswhere1) { $vswhere = $vswhere1 } elseif (Test-Path $vswhere2) { $vswhere = $vswhere2 }

if (-not $vswhere) {
  Write-Error "Nie znaleziono vswhere.exe. Zainstaluj Visual Studio 2022 (z MSBuild)."
  exit 1
}

$installPath = & $vswhere -latest -requires Microsoft.Component.MSBuild -property installationPath
if (-not $installPath) {
  Write-Error "Nie znaleziono instalacji Visual Studio 2022 z MSBuild."
  exit 1
}

$msbuild = Join-Path $installPath "MSBuild\Current\Bin\MSBuild.exe"
if (-not (Test-Path $msbuild)) {
  Write-Error "Nie znaleziono MSBuild.exe w $msbuild"
  exit 1
}

$solution = "Abs.Ssms.TabColoring.sln"
Write-Host "Buduję $solution ($Configuration) przy użyciu $msbuild"
& $msbuild $solution /t:Restore,Build /p:Configuration=$Configuration

# Find VSIX
$vsix = Get-ChildItem -Recurse -Filter *.vsix | Select-Object -First 1
if ($vsix) {
  Write-Host "Zbudowano VSIX: $($vsix.FullName)"
} else {
  Write-Warning "Nie znaleziono pliku .vsix. Sprawdź logi MSBuild, czy projekt zapakował rozszerzenie."
}
