# Fix encoding for OrderTests.cs
$ErrorActionPreference = "Stop"

$filePath = "TestProject1/Tests/OrderTests.cs"
Write-Host "Reading file: $filePath"

# Read file as bytes then decode as UTF-8
$bytes = [System.IO.File]::ReadAllBytes($filePath)
$content = [System.Text.Encoding]::UTF8.GetString($bytes)

Write-Host "Original length: $($content.Length)"

# Fix encoding issues
$replacements = @{
    'ÄĂ£ Hủy' = 'Đã hủy'
    'ÄĂ£ há»§y' = 'Đã hủy'
    'Sá»'' = 'Số'
    'Ä'Æ¡n hĂ ng' = 'đơn hàng'
    'Ä'Ă£' = 'đã'
    'hiá»ƒn thá»‹' = 'hiển thị'
    'Ä'Ăºng' = 'đúng'
    'Ä'ang giao' = 'đang giao'
    'rá»—ng' = 'rỗng'
    'Ä'Æ°á»£c' = 'được'
    'táº¡o' = 'tạo'
    'má»›i' = 'mới'
    'thĂ nh cĂ´ng' = 'thành công'
    'VĂ o' = 'Vào'
    'chá»n' = 'chọn'
    'Kiá»ƒm tra' = 'Kiểm tra'
    'váº«n' = 'vẫn'
    'cĂ³' = 'có'
    'xuáº¥t hiá»‡n' = 'xuất hiện'
    'dĂ¹' = 'dù'
    'Chá»' = 'Chờ'
    'láº¥y hĂ ng' = 'lấy hàng'
    'xĂ¡c nháº­n' = 'xác nhận'
    'tráº¡ng thĂ¡i' = 'trạng thái'
    'Há»§y' = 'Hủy'
    'há»§y' = 'hủy'
    'Ä'ang xá»­ lĂ½' = 'đang xử lý'
}

foreach ($key in $replacements.Keys) {
    $content = $content.Replace($key, $replacements[$key])
}

Write-Host "Fixed length: $($content.Length)"

# Write back as UTF-8
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText($filePath, $content, $utf8NoBom)

Write-Host "File fixed successfully!"
