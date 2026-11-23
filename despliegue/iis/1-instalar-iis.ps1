# ---------------------------------------------------------------------------
# Paso 1. Activa IIS y comprueba el modulo de ASP.NET Core.
#
# REQUIERE CONSOLA DE ADMINISTRADOR.
#   Boton derecho en Terminal o PowerShell -> Ejecutar como administrador
# ---------------------------------------------------------------------------
$ErrorActionPreference = 'Stop'

$principal = New-Object Security.Principal.WindowsPrincipal(
    [Security.Principal.WindowsIdentity]::GetCurrent())

if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host 'Esta consola no esta elevada. Abra PowerShell como administrador.' -ForegroundColor Red
    exit 1
}

# Caracteristicas minimas para hospedar ASP.NET Core. No hacen falta las de
# ASP.NET clasico (ISAPI, NetFxExtensibility): el modulo de ASP.NET Core no
# usa el CLR de IIS, lanza su propio proceso de .NET.
$caracteristicas = @(
    'IIS-WebServerRole',
    'IIS-WebServer',
    'IIS-CommonHttpFeatures',
    'IIS-StaticContent',
    'IIS-DefaultDocument',
    'IIS-HttpErrors',
    'IIS-HttpLogging',
    'IIS-Security',
    'IIS-RequestFiltering',
    'IIS-WebServerManagementTools',
    'IIS-ManagementConsole'
)

Write-Host 'Activando IIS...' -ForegroundColor Cyan
foreach ($c in $caracteristicas) {
    $estado = Get-WindowsOptionalFeature -Online -FeatureName $c
    if ($estado.State -eq 'Enabled') {
        Write-Host "  ya estaba: $c"
    } else {
        Enable-WindowsOptionalFeature -Online -FeatureName $c -All -NoRestart | Out-Null
        Write-Host "  activada : $c" -ForegroundColor Green
    }
}

Write-Host ''
Write-Host '=== comprobaciones ===' -ForegroundColor Cyan
$w3 = Get-Service W3SVC -ErrorAction SilentlyContinue
Write-Host "  servicio W3SVC : $(if ($w3) { $w3.Status } else { 'NO EXISTE' })"
Write-Host "  appcmd.exe     : $(Test-Path 'C:\Windows\System32\inetsrv\appcmd.exe')"

# ---------------------------------------------------------------------------
# El Hosting Bundle es lo que ensena a IIS a hospedar ASP.NET Core. Sin el,
# los sitios devuelven 500.19 o 500.21 y el error no dice por que.
# ---------------------------------------------------------------------------
$modulo = 'C:\Program Files\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll'

if (Test-Path $modulo) {
    Write-Host "  Hosting Bundle : instalado (version $((Get-Item $modulo).VersionInfo.FileVersion))" -ForegroundColor Green
} else {
    Write-Host ''
    Write-Host 'FALTA EL HOSTING BUNDLE DE ASP.NET CORE.' -ForegroundColor Red
    Write-Host 'Descarguelo de:' -ForegroundColor Yellow
    Write-Host '  https://dotnet.microsoft.com/download/dotnet/10.0' -ForegroundColor Yellow
    Write-Host 'En la columna Windows, busque "Hosting Bundle" y ejecute el instalador.'
    Write-Host 'Tambien puede buscarlo con:  winget search dotnet hosting'
    Write-Host ''
    Write-Host 'IMPORTANTE: instalelo DESPUES de activar IIS. Si se instala antes,'
    Write-Host 'no registra el modulo en IIS y hay que repararlo desde'
    Write-Host 'Panel de control -> Programas -> Reparar.'
    Write-Host ''
    Write-Host 'Al terminar, vuelva a ejecutar este script para confirmar.'
    exit 1
}

Write-Host ''
Write-Host 'Paso 1 completo. Siga con 2-publicar.ps1 (sin administrador).' -ForegroundColor Green
