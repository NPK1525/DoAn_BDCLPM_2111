#!/usr/bin/env python3
# -*- coding: utf-8 -*-

import codecs

# Đọc file với encoding hiện tại
with open('TestProject1/Tests/OrderTests.cs', 'r', encoding='utf-8', errors='ignore') as f:
    content = f.read()

# Fix các lỗi encoding phổ biến
replacements = {
    'ÄĂ£ Hủy': 'Đã hủy',
    'ÄĂ£ há»§y': 'Đã hủy',
    'Sá»'': 'Số',
    'Ä'Æ¡n hĂ ng': 'đơn hàng',
    'Ä'Ă£': 'đã',
    'hiá»ƒn thá»‹': 'hiển thị',
    'Ä'Ăºng': 'đúng',
    'Ä'ang giao': 'đang giao',
    'rá»—ng': 'rỗng',
    'Ä'Æ°á»£c': 'được',
    'táº¡o': 'tạo',
    'má»›i': 'mới',
    'thĂ nh cĂ´ng': 'thành công',
    'VĂ o': 'Vào',
    'chá»n': 'chọn',
    'Kiá»ƒm tra': 'Kiểm tra',
    'váº«n': 'vẫn',
    'cĂ³': 'có',
    'xuáº¥t hiá»‡n': 'xuất hiện',
    'dĂ¹': 'dù',
    'Chá»': 'Chờ',
    'láº¥y hĂ ng': 'lấy hàng',
    'xĂ¡c nháº­n': 'xác nhận',
    'tráº¡ng thĂ¡i': 'trạng thái',
    'Há»§y': 'Hủy',
    'há»§y': 'hủy',
    'Ä'ang xá»­ lĂ½': 'đang xử lý',
}

for old, new in replacements.items():
    content = content.replace(old, new)

# Ghi lại file với UTF-8
with open('TestProject1/Tests/OrderTests.cs', 'w', encoding='utf-8', newline='\r\n') as f:
    f.write(content)

print("Fixed encoding successfully!")
