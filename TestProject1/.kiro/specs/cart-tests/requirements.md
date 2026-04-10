# Requirements Document

## Introduction

Tính năng này bao gồm các Selenium test cases (NUnit + C#) kiểm thử chức năng Giỏ hàng (Cart) của web shop tại `https://shop-production-b6d0.up.railway.app`. Các test cases bao phủ 4 nhóm chức năng: hiển thị giỏ hàng, thêm sản phẩm, cập nhật số lượng, và xóa sản phẩm. Tất cả test kế thừa `BaseTest`, sử dụng `DriverFactory.InitDriver()` và `LoginPage` đã có sẵn.

## Glossary

- **CartTests**: Lớp test NUnit chứa toàn bộ test cases cho chức năng giỏ hàng, kế thừa `BaseTest`.
- **CartPage**: Page Object đại diện cho trang giỏ hàng (`/Cart`), cung cấp các thao tác và assertion trên giỏ hàng.
- **ProductDetailsPage**: Page Object đại diện cho trang chi tiết sản phẩm, cung cấp thao tác chọn màu, size, số lượng và thêm vào giỏ.
- **BaseTest**: Lớp cơ sở cung cấp `driver`, `baseUrl`, setup/teardown và chụp screenshot khi test thất bại.
- **LoginPage**: Page Object đã có sẵn, cung cấp phương thức `Login(email, password)` và `IsLoginSuccess()`.
- **DriverFactory**: Utility khởi tạo ChromeDriver qua `InitDriver()`.
- **Cart_Item**: Một dòng sản phẩm trong giỏ hàng, bao gồm hình ảnh, tên, màu, size, đơn giá, số lượng, thành tiền và nút xóa.
- **Stock_Limit**: Số lượng tồn kho tối đa của một sản phẩm theo màu và size.

---

## Requirements

### Requirement 1: Hiển thị giỏ hàng (F3.1)

**User Story:** As a khách hàng đã đăng nhập, I want xem nội dung giỏ hàng của mình, so that tôi có thể kiểm tra các sản phẩm trước khi đặt hàng.

#### Acceptance Criteria

1. WHEN người dùng truy cập trang giỏ hàng với ít nhất một sản phẩm, THE CartPage SHALL hiển thị danh sách Cart_Item bao gồm hình ảnh, tên sản phẩm, đơn giá, nút tăng số lượng (+), nút giảm số lượng (-), thành tiền, và nút xóa cho mỗi dòng.
2. WHEN người dùng truy cập trang giỏ hàng với ít nhất một sản phẩm, THE CartPage SHALL hiển thị nút "Đặt hàng" để tiến hành thanh toán.
3. WHEN người dùng truy cập trang giỏ hàng khi giỏ hàng rỗng, THE CartPage SHALL hiển thị thông báo giỏ hàng trống.
4. WHEN người dùng truy cập trang giỏ hàng khi giỏ hàng rỗng, THE CartPage SHALL hiển thị nút "Tiếp tục mua sắm" dẫn về trang chủ.

---

### Requirement 2: Thêm sản phẩm vào giỏ hàng (F3.2)

**User Story:** As a khách hàng đã đăng nhập, I want thêm sản phẩm vào giỏ hàng, so that tôi có thể mua sản phẩm đó.

#### Acceptance Criteria

1. WHEN người dùng chọn màu, size hợp lệ và số lượng hợp lệ rồi nhấn "Thêm vào giỏ", THE ProductDetailsPage SHALL thêm sản phẩm mới vào giỏ hàng và THE CartPage SHALL hiển thị Cart_Item tương ứng với màu, size và số lượng đã chọn.
2. WHEN người dùng thêm một sản phẩm đã có trong giỏ hàng với cùng màu và size, THE CartPage SHALL cộng dồn số lượng vào Cart_Item hiện có thay vì tạo dòng mới.
3. WHEN người dùng thêm một sản phẩm đã có trong giỏ hàng nhưng khác màu hoặc khác size, THE CartPage SHALL tạo một Cart_Item mới riêng biệt.
4. WHEN người dùng nhập số lượng vượt quá Stock_Limit (ví dụ: 999) và nhấn "Thêm vào giỏ", THE ProductDetailsPage SHALL hiển thị thông báo lỗi vượt tồn kho và THE CartPage SHALL không thêm số lượng vượt quá Stock_Limit.
5. WHEN người dùng chưa chọn size và nhấn "Thêm vào giỏ", THE ProductDetailsPage SHALL hiển thị thông báo yêu cầu chọn size.
6. WHEN người dùng chưa chọn màu và nhấn "Thêm vào giỏ", THE ProductDetailsPage SHALL hiển thị thông báo yêu cầu chọn màu.
7. WHEN người dùng nhập số lượng bằng 0 và nhấn "Thêm vào giỏ", THE ProductDetailsPage SHALL không thêm sản phẩm vào giỏ hàng.
8. WHEN người dùng nhập số lượng âm (ví dụ: -1) và nhấn "Thêm vào giỏ", THE ProductDetailsPage SHALL không thêm sản phẩm vào giỏ hàng.

---

### Requirement 3: Cập nhật số lượng trong giỏ hàng (F3.3)

**User Story:** As a khách hàng đã đăng nhập, I want thay đổi số lượng sản phẩm trong giỏ hàng, so that tôi có thể điều chỉnh đơn hàng trước khi thanh toán.

#### Acceptance Criteria

1. WHEN người dùng nhấn nút (+) trên một Cart_Item có số lượng hiện tại là N (N < Stock_Limit), THE CartPage SHALL tăng số lượng Cart_Item đó lên N+1 và cập nhật thành tiền tương ứng.
2. WHEN người dùng nhấn nút (-) trên một Cart_Item có số lượng hiện tại là N (N > 1), THE CartPage SHALL giảm số lượng Cart_Item đó xuống N-1 và cập nhật thành tiền tương ứng.
3. WHEN người dùng cập nhật số lượng Cart_Item về 0, THE CartPage SHALL xóa Cart_Item đó khỏi giỏ hàng.
4. WHEN người dùng nhập số lượng âm (ví dụ: -10) vào ô số lượng của Cart_Item, THE CartPage SHALL không cập nhật số lượng về giá trị âm.
5. WHEN người dùng nhập số lượng vượt quá Stock_Limit (ví dụ: 999) vào ô số lượng của Cart_Item, THE CartPage SHALL hiển thị thông báo lỗi vượt tồn kho và không cập nhật số lượng vượt quá Stock_Limit.
6. WHEN người dùng nhập ký tự chữ (ví dụ: "abc") vào ô số lượng của Cart_Item, THE CartPage SHALL không cập nhật số lượng và duy trì giá trị hợp lệ trước đó.
7. WHEN người dùng nhập số thập phân (ví dụ: "1.5") vào ô số lượng của Cart_Item, THE CartPage SHALL không cập nhật số lượng về giá trị thập phân.
8. WHEN người dùng nhập chuỗi có khoảng trắng (ví dụ: "1 0") vào ô số lượng của Cart_Item, THE CartPage SHALL không cập nhật số lượng về giá trị không hợp lệ đó.
9. WHEN người dùng nhấn nút (+) trên một Cart_Item có số lượng hiện tại bằng Stock_Limit, THE CartPage SHALL hiển thị thông báo đã đạt giới hạn tồn kho và không tăng thêm số lượng.
10. WHEN người dùng nhấn nút (-) trên một Cart_Item có số lượng hiện tại là 1, THE CartPage SHALL xóa Cart_Item đó khỏi giỏ hàng.

---

### Requirement 4: Xóa sản phẩm khỏi giỏ hàng (F3.4)

**User Story:** As a khách hàng đã đăng nhập, I want xóa sản phẩm khỏi giỏ hàng, so that tôi có thể loại bỏ sản phẩm không muốn mua.

#### Acceptance Criteria

1. WHEN người dùng nhấn nút xóa (icon thùng rác) trên một Cart_Item, THE CartPage SHALL xóa Cart_Item đó khỏi danh sách và cập nhật tổng tiền.
2. WHEN người dùng xóa tất cả Cart_Item khỏi giỏ hàng, THE CartPage SHALL hiển thị trạng thái giỏ hàng rỗng theo Requirement 1 Acceptance Criteria 3 và 4.

---

### Requirement 5: Cấu trúc và hạ tầng test (Infrastructure)

**User Story:** As a developer, I want các test cases được tổ chức theo chuẩn dự án hiện tại, so that dễ bảo trì và tích hợp vào CI/CD.

#### Acceptance Criteria

1. THE CartTests SHALL kế thừa `BaseTest` và sử dụng `DriverFactory.InitDriver()` trong `[SetUp]`.
2. THE CartTests SHALL sử dụng `LoginPage.Login("shopclothing1525@gmail.com", "Test@123")` để đăng nhập trước các test cần xác thực.
3. THE CartPage SHALL kế thừa `BasePage` và đóng gói toàn bộ thao tác trên trang `/Cart`.
4. THE ProductDetailsPage SHALL kế thừa `BasePage` và đóng gói thao tác chọn màu, size, số lượng và thêm vào giỏ hàng.
5. WHEN một test thất bại, THE BaseTest SHALL tự động chụp screenshot và lưu vào thư mục `Screenshots/` với tên theo định dạng `{TestName}_{yyyyMMdd_HHmmss}.png`.
