# Script xoa tat ca screenshot cu

$screenshotDir = "TestProject1\Screenshots"

if (Test-Path $screenshotDir) {
    Write-Host "Dang xoa screenshots cu..." -ForegroundColor Yellow
    Remove-Item "$screenshotDir\*.png" -Force
    Write-Host "✓ Da xoa xong!" -ForegroundColor Green
} else {
    Write-Host "Thu muc Screenshots khong ton tai" -ForegroundColor Red
}
