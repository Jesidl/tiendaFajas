# ---------------------------------------------------------------------------
# Paso 3. Crea los dos sitios en IIS con sus grupos de aplicaciones.
#
# REQUIERE CONSOLA DE ADMINISTRADOR.
#
# Los puertos son los mismos que usa el entorno de desarrollo: 5250 para el
# servicio y 5150 para el front. No es casualidad. El front tiene la direccion
# del servicio en presentacion_tiendaFajas/nucleo/Servidor.cs, y el atributo
# src de las imagenes apunta ahi. Manteniendo los puertos, pasar a IIS no
# exige ningun cambio de codigo.
# ---------------------------------------------------------------------------
$ErrorActionPreference = 'Stop'

$principal = New-Object Security.Principal.WindowsPrincipal(
    [Security.Principal.WindowsIdentity]::GetCurrent())

if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host 'Esta consola no esta elevada. Abra PowerShell como administrador.' -ForegroundColor Red
    exit 1
}

Import-Module WebAdministration -ErrorAction Stop

$destino = 'E:\publicado\tiendaFajas'

$sitios = @(
    @{ Nombre = 'tiendaFajas-servicio'; Puerto = 5250; Carpeta = "$destino\servicio" },
    @{ Nombre = 'tiendaFajas-front';    Puerto = 5150; Carpeta = "$destino\front" }
)

foreach ($s in $sitios) {
    if (-not (Test-Path $s.Carpeta)) {
        throw "No existe $($s.Carpeta). Ejecute antes 2-publicar.ps1"
    }
}

# Los procesos de desarrollo ocupan esos puertos y el sitio de IIS no podria
# arrancar.
foreach ($nombre in @('servicio_tiendaFajas', 'front_tiendaFajas')) {
    Get-Process -Name $nombre -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host "  deteniendo el proceso de desarrollo $nombre ($($_.Id))" -ForegroundColor Yellow
        Stop-Process -Id $_.Id -Force
    }
}

foreach ($s in $sitios) {
    $nombre = $s.Nombre
    Write-Host ''
    Write-Host "=== $nombre ===" -ForegroundColor Cyan

    # ---------- grupo de aplicaciones ----------
    if (Test-Path "IIS:\AppPools\$nombre") {
        Remove-WebAppPool -Name $nombre
        Write-Host '  grupo anterior eliminado'
    }

    New-WebAppPool -Name $nombre | Out-Null

    # "Sin codigo administrado". ASP.NET Core no usa el CLR que IIS carga por
    # omision: el modulo lanza un proceso de dotnet aparte. Si se deja el
    # valor por omision (v4.0), el sitio responde 500.30 o 502.5.
    Set-ItemProperty "IIS:\AppPools\$nombre" -Name managedRuntimeVersion -Value ''

    # ApplicationPoolIdentity es una cuenta virtual por grupo, sin permisos
    # de mas. Es la razon por la que hay que crearle un inicio de sesion en
    # SQL Server: ver 4-permisos-sql.sql
    Set-ItemProperty "IIS:\AppPools\$nombre" -Name processModel.identityType -Value 'ApplicationPoolIdentity'

    # El sitio arranca con Windows y no espera la primera peticion.
    Set-ItemProperty "IIS:\AppPools\$nombre" -Name startMode -Value 'AlwaysRunning'

    Write-Host '  grupo creado: sin codigo administrado, ApplicationPoolIdentity' -ForegroundColor Green

    # ---------- sitio ----------
    if (Test-Path "IIS:\Sites\$nombre") {
        Remove-Website -Name $nombre
        Write-Host '  sitio anterior eliminado'
    }

    New-Website -Name $nombre -Port $s.Puerto -PhysicalPath $s.Carpeta `
        -ApplicationPool $nombre | Out-Null

    # El proceso se mantiene cargado en vez de dormirse por inactividad.
    Set-ItemProperty "IIS:\Sites\$nombre" -Name applicationDefaults.preloadEnabled -Value $true

    Write-Host "  sitio creado en el puerto $($s.Puerto) -> $($s.Carpeta)" -ForegroundColor Green

    # ---------- permisos del sistema de archivos ----------
    # La identidad del grupo necesita leer la carpeta publicada, y escribir en
    # logs para poder dejar el motivo de un fallo de arranque.
    $identidad = "IIS AppPool\$nombre"
    icacls $s.Carpeta /grant "${identidad}:(OI)(CI)(RX)" /T /Q | Out-Null
    icacls (Join-Path $s.Carpeta 'logs') /grant "${identidad}:(OI)(CI)(M)" /T /Q | Out-Null
    Write-Host "  permisos otorgados a $identidad" -ForegroundColor Green
}

Write-Host ''
Write-Host '=== estado ===' -ForegroundColor Cyan
Get-Website | Where-Object { $_.Name -like 'tiendaFajas*' } |
    Format-Table Name, State, PhysicalPath, @{n='Enlace';e={$_.bindings.Collection.bindingInformation}} -AutoSize

Write-Host ''
Write-Host 'Paso 3 completo.' -ForegroundColor Green
Write-Host 'FALTA EL PASO 4: sin el inicio de sesion de SQL Server para la' -ForegroundColor Yellow
Write-Host 'identidad del grupo, el servicio arranca pero toda consulta falla.' -ForegroundColor Yellow
