# ABS.SSMS.TabColoring

## Wymagania
- Visual Studio 2022 (17.x) z workloadem **Visual Studio extension development** i **.NET desktop development**.
- .NET Framework 4.8.
- SQL Server Management Studio 21 (x64).

## Budowa lokalna
1. Otwórz `Abs.Ssms.TabColoring.sln` w VS 2022.
2. Zbuduj w konfiguracji **Release** — w katalogu `src/Abs.Ssms.TabColoring/bin/Release/` pojawi się plik `.vsix`.

### Alternatywnie: skrypt PowerShell (1 komenda)
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
./build.ps1
```
Po zakończeniu skrypt wypisze ścieżkę do wygenerowanego `.vsix`.

## Instalacja
- Zamknij SSMS, uruchom plik `.vsix`, wybierz **SQL Server Management Studio 21 (x64)** i kliknij *Install*.
- Po restarcie skonfiguruj reguły: **Tools → Options → ABS → Tab Coloring**.
