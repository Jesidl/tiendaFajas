# ---------------------------------------------------------------------------
# Paso 5. Comprueba que los dos sitios de IIS responden y que el servicio
# llega a la base de datos.
#
# NO requiere administrador.
# ---------------------------------------------------------------------------
$servicio = 'http://localhost:5250'
$front    = 'http://localhost:5150'
$usuario  = 'admin@tiendafajas.com'
$fallos   = 0

function Write-Resultado([string]$que, [bool]$bien, [string]$detalle) {
    $marca = if ($bien) { 'OK  ' } else { 'FALLA' }
    $color = if ($bien) { 'Green' } else { 'Red' }
    Write-Host ("  {0,-5} {1,-46} {2}" -f $marca, $que, $detalle) -ForegroundColor $color
    if (-not $bien) { $script:fallos++ }
}

Write-Host '=== el servicio responde ===' -ForegroundColor Cyan
try {
    $r = Invoke-WebRequest "$servicio/Cargos/Consultar" -Method Post -Body '{}' `
         -ContentType 'application/json' -Headers @{ 'X-Usuario' = $usuario } `
         -TimeoutSec 30 -ErrorAction Stop
    Write-Resultado 'POST Cargos/Consultar' ($r.StatusCode -eq 200) "HTTP $($r.StatusCode)"

    $filas = ($r.Content | ConvertFrom-Json | Measure-Object).Count
    Write-Resultado 'llega a SQL Server (devuelve filas)' ($filas -gt 0) "$filas filas"
}
catch {
    Write-Resultado 'POST Cargos/Consultar' $false $_.Exception.Message
    Write-Host ''
    Write-Host '  Si el error es 500.30, lea el motivo real en:' -ForegroundColor Yellow
    Write-Host '    E:\publicado\tiendaFajas\servicio\logs\stdout_*.log' -ForegroundColor Yellow
    Write-Host '  Si el error menciona el inicio de sesion, falta el paso 4.' -ForegroundColor Yellow
}

Write-Host ''
Write-Host '=== el front responde ===' -ForegroundColor Cyan
try {
    $r = Invoke-WebRequest $front -TimeoutSec 30 -ErrorAction Stop -SessionVariable s
    Write-Resultado 'GET /' ($r.StatusCode -eq 200) "HTTP $($r.StatusCode)"
    Write-Resultado 'muestra el formulario de ingreso' ($r.Content -match 'Ingreso') ''

    if ($r.Content -match 'name="__RequestVerificationToken"[^>]*value="([^"]+)"') {
        $token = $Matches[1]
        $r = Invoke-WebRequest "$front/?handler=BtEntrar" -Method Post -WebSession $s `
             -TimeoutSec 30 -Body @{
                 Correo                     = $usuario
                 ClaveEscrita               = 'Admin.123'
                 __RequestVerificationToken = $token
             }
        Write-Resultado 'el login funciona end to end' ($r.Content -match 'Bienvenido') ''

        $r = Invoke-WebRequest "$front/Ventanas/Productos" -WebSession $s -TimeoutSec 30
        $celdas = ([regex]::Matches($r.Content, '<td>')).Count
        Write-Resultado 'la pantalla de productos trae datos' ($celdas -gt 0) "$celdas celdas"
    }
    else {
        Write-Resultado 'token antifalsificacion presente' $false 'no se encontro'
    }
}
catch {
    Write-Resultado 'GET /' $false $_.Exception.Message
}

Write-Host ''
Write-Host '=== sitios en IIS ===' -ForegroundColor Cyan
try {
    Import-Module WebAdministration -ErrorAction Stop
    Get-Website | Where-Object { $_.Name -like 'tiendaFajas*' } |
        ForEach-Object { Write-Host "  $($_.Name): $($_.State)" }
}
catch {
    Write-Host '  (para ver el estado hace falta una consola de administrador)'
}

Write-Host ''
if ($fallos -eq 0) {
    Write-Host 'Todo responde. La aplicacion esta publicada en IIS.' -ForegroundColor Green
} else {
    Write-Host "$fallos comprobaciones fallaron. Vea el README de esta carpeta." -ForegroundColor Red
}
