# Implementation Plan: Order Delivery List View

## Overview

Triển khai test case TC_F4.1_01 để kiểm thử tính năng xem danh sách đơn hàng đang giao. Tạo OrdersPage.cs (page object) và OrderTests.cs (test class) tuân theo mô hình Page Object Model đã có trong dự án, sử dụng các patterns từ CartPage và CartTests.

## Tasks

- [x] 1. Tạo OrdersPage.cs với các phương thức cơ bản
  - Tạo file `TestProject1/Pages/OrdersPage.cs`
  - Kế thừa từ `BasePage` với primary constructor syntax
  - Implement phương thức `NavigateTo()` để điều hướng đến trang Orders
  - Implement phương thức `IsOnOrdersPage()` để verify đang ở trang Orders
  - Sử dụng namespace `SeleniumTests.Pages` để nhất quán với các page objects khác
  - _Requirements: 1.1_

- [x] 2. Implement các phương thức chọn và verify delivery section
  - [x] 2.1 Thêm phương thức chọn delivery section
    - Implement `SelectDeliverySection()` với multiple fallback locators (data attributes, text-based XPath)
    - Sử dụng `WaitForElementClickable()` từ BasePage trước khi click
    - Log console để debug khi không tìm thấy element
    - _Requirements: 2.1_
  
  - [x] 2.2 Thêm các phương thức verification cho delivery section
    - Implement `IsDeliverySectionDisplayed()` để kiểm tra section hiển thị
    - Implement `IsDeliverySectionSelected()` để kiểm tra section được chọn
    - Implement `HasDeliveryOrders()` để kiểm tra có đơn hàng nào không
    - Implement `GetDeliveryOrderCount()` để đếm số đơn hàng
    - Sử dụng `IsDisplayed()` từ BasePage với timeout ngắn
    - _Requirements: 2.1, 2.5_

- [x] 3. Implement các phương thức lấy thông tin order items
  - [x] 3.1 Thêm phương thức lấy thông tin cơ bản của order
    - Implement `GetOrderId(int index = 0)` để lấy mã đơn hàng
    - Implement `GetOrderDate(int index = 0)` để lấy ngày đặt hàng
    - Implement `GetDeliveryStatus(int index = 0)` để lấy trạng thái giao hàng
    - Sử dụng `GetText()` từ BasePage với fallback locators
    - _Requirements: 2.2, 2.3, 2.4_
  
  - [x] 3.2 Thêm các phương thức kiểm tra hiển thị thông tin
    - Implement `IsOrderIdDisplayed(int index = 0)` để verify order ID hiển thị
    - Implement `IsOrderDateDisplayed(int index = 0)` để verify ngày hiển thị
    - Implement `IsDeliveryStatusDisplayed(int index = 0)` để verify status hiển thị
    - Trả về boolean, sử dụng try-catch để handle missing elements
    - _Requirements: 2.2, 2.3, 2.4_

- [x] 4. Implement các phương thức xử lý loading và error states
  - Implement `IsLoadingIndicatorDisplayed()` để kiểm tra loading indicator
  - Implement `IsEmptyStateDisplayed()` để kiểm tra empty state
  - Implement `GetEmptyStateMessage()` để lấy nội dung empty state message
  - Sử dụng multiple locator strategies với timeout ngắn
  - _Requirements: 2.5, 5.1, 5.2, 6.1, 6.2_

- [x] 5. Tạo OrderTests.cs với test case TC_F4_1_01
  - [x] 5.1 Tạo test class structure
    - Tạo file `TestProject1/Tests/OrderTests.cs`
    - Kế thừa từ `BaseTest`
    - Thêm constants cho test credentials (TestEmail, TestPassword)
    - Sử dụng namespace `SeleniumTests.Tests` để nhất quán
    - Override `Setup()` để khởi tạo driver với `DriverFactory.InitDriver()`
    - _Requirements: 1.1, 1.3_
  
  - [x] 5.2 Implement helper method LoginAsTestUser
    - Tạo private method `LoginAsTestUser()` để đăng nhập với test account
    - Sử dụng `LoginPage` để thực hiện login
    - Assert login thành công trước khi tiếp tục
    - _Requirements: 1.3_
  
  - [x] 5.3 Implement test case TC_F4_1_01
    - Tạo test method với attribute `[Test]`
    - Gọi `LoginAsTestUser()` để đăng nhập
    - Navigate đến Orders page
    - Select delivery section
    - Verify delivery section được hiển thị và chọn
    - Sử dụng `Assert.Multiple()` để gom tất cả assertions
    - Verify có ít nhất một order hoặc empty state message
    - Verify mỗi order hiển thị đầy đủ: Order ID, Date, Status
    - Thêm Console.WriteLine() để log các bước quan trọng
    - _Requirements: 1.1, 1.2, 2.1, 2.2, 2.3, 2.4, 3.1_

- [x] 6. Checkpoint - Chạy test và verify kết quả
  - Chạy test TC_F4_1_01 một lần
  - Verify tất cả assertions pass
  - Kiểm tra screenshot được capture nếu test fail
  - Điều chỉnh locators nếu cần thiết dựa trên cấu trúc HTML thực tế
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks không có dấu `*` là bắt buộc và sẽ được implement
- Test case TC_F4_1_01 tuân theo single execution policy (chỉ chạy một lần)
- OrdersPage sử dụng multiple fallback locators để tăng độ bền vững với UI changes
- Test phụ thuộc vào dữ liệu có sẵn trong hệ thống (test account phải có đơn hàng đang giao)
- Tất cả tasks đều reference đến requirements cụ thể để đảm bảo traceability
