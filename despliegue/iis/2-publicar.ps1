# ---------------------------------------------------------------------------
# Paso 2. Compila los dos procesos en modo Release y los deja listos para IIS.
#
# NO requiere administrador.
#
# La carpeta de salida esta fuera del repositorio a proposito: IIS no debe
# servir el arbol de codigo fuente.
# ---------------------------------------------------------------------------
$ErrorActionPreference = 'Stop'

$repo    = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$destino = 'E:\publicado\tiendaFajas'

Write-Host "repositorio : $repo"
Write-Host "destino     : $destino"
Write-Host ''

# Los procesos de desarrollo bloquean los DLL y la publicacion falla con
# MSB3027. Se detienen antes.
foreach ($nombre in @('servicio_tiendaFajas', 'front_tiendaFajas')) {
    Get-Process -Name $nombre -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host "  deteniendo $nombre ($($_.Id))" -ForegroundColor Yellow
        Stop-Process -Id $_.Id -Force
    }
}

$proyectos = @(
    @{ Nombre = 'servicio'; Ruta = 'backend\servicio_tiendaFajas\servicio_tiendaFajas.csproj' },
    @{ Nombre = 'front';    Ruta = 'frontend\front_tiendaFajas\front_tiendaFajas.csproj' }
)

foreach ($p in $proyectos) {
    $salida = Join-Path $destino $p.Nombre

    Write-Host "publicando $($p.Nombre)..." -ForegroundColor Cyan
    dotnet publish (Join-Path $repo $p.Ruta) -c Release -o $salida --nologo
    if ($LASTEXITCODE -ne 0) { throw "fallo la publicacion de $($p.Nombre)" }

    # -----------------------------------------------------------------------
    # Se activa el registro de la salida estandar.
    #
    # dotnet publish lo deja desactivado. Cuando una aplicacion de ASP.NET
    # Core no arranca bajo IIS, el navegador muestra un 500.30 que no dice
    # nada: ni la excepcion, ni el archivo, ni la linea. Con esto activado, el
    # motivo real queda escrito en logs\stdout.
    #
    # Hay que repetirlo despues de cada publicacion, porque el web.config se
    # regenera.
    # -----------------------------------------------------------------------
    $config = Join-Path $salida 'web.config'
    $xml = [xml](Get-Content $config)
    $nodo = $xml.configuration.location.'system.webServer'.aspNetCore
    $nodo.stdoutLogEnabled = 'true'
    $xml.Save($config)

    New-Item -ItemType Directory -Force -Path (Join-Path $salida 'logs') | Out-Null

    $n = (Get-ChildItem $salida -Recurse -File | Measure-Object).Count
    $mb = [math]::Round(((Get-ChildItem $salida -Recurse -File |
            Measure-Object -Property Length -Sum).Sum / 1MB), 1)
    Write-Host "  $($p.Nombre): $n archivos, $mb MB, registro de arranque activado" -ForegroundColor Green
}

Write-Host ''
Write-Host 'Paso 2 completo. Siga con 3-crear-sitios.ps1 (como administrador).' -ForegroundColor Green
