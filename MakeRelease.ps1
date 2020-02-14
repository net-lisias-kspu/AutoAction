$name = "AutoAction"
$srcDir = "GameData"
$dstDir = "Releases"

$verFile = "$srcDir\$name\$name.version"
$verJson = Get-Content $verFile -Raw | ConvertFrom-Json
$ver = $verJson.VERSION
$ksp = $verJson.KSP_VERSION
$verString = "$($ver.MAJOR).$($ver.MINOR).$($ver.PATCH)"
$kspString = "_ksp$($ksp.MAJOR).$($ksp.MINOR).$($ksp.PATCH)"

$zipFile = "$dstDir\AutoActions$verString$kspString.zip"
Write-Host $zipFile

If (Test-Path $zipFile) {
  Read-Host "Press Enter to overwrite or Ctrl+Break to quit"
}

Compress-Archive $srcDir $zipFile -Force

