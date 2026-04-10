# Design Document: Order Delivery List View

## Overview

Tài liệu thiết kế này mô tả cách triển khai test case TC_F4.1_01 để kiểm thử tính năng xem danh sách đơn hàng đang giao. Test case này sử dụng Selenium WebDriver với C# và NUnit framework, tuân theo mô hình Page Object Model (POM) đã được thiết lập trong dự án.

Mục tiêu chính là tạo một test tự động hóa để xác minh rằng người dùng đã đăng nhập có thể:
- Truy cập trang Orders
- Chọn phần "orders being delivered"
- Xem danh sách các đơn hàng đang được giao hàng với đầy đủ thông tin

## Architecture

### Test Architecture

Test case này sẽ tuân theo kiến trúc 3 lớp đã được sử dụng trong dự án:

1. **Test Layer** (`Tests/OrderTests.cs`): Chứa test case TC_F4_1_01
2. **Page Object Layer** (`Pages/OrdersPage.cs`): Đóng gói các tương tác với trang Orders
3. **Base Layer** (`Tests/BaseTest.cs`, `Pages/BasePage.cs`): Cung cấp các chức năng chung

### Test Flow

```
[Setup] → [Login] → [Navigate to Orders Page] → [Select Delivery Section] → [Verify List Display] → [TearDown]
```

### Dependencies

- Selenium WebDriver
- NUnit Framework
- Existing Page Objects: LoginPage, BasePage
- Existing Test Infrastructure: BaseTest, DriverFactory

## Components and Interfaces

### 1. OrdersPage (Page Object)

**Responsibility**: Đóng gói tất cả các tương tác với trang Orders và phần danh sách đơn hàng đang giao.

**Key Methods**:

```csharp
public class OrdersPage : BasePage
{
    // Navigation
    public void NavigateTo()
    
    // Section Selection
    public void SelectDeliverySection()
    
    // Verification Methods
    public bool IsOnOrdersPage()
    public bool IsDeliverySectionDisplayed()
    public bool IsDeliverySectionSelected()
    public int GetDeliveryOrderCount()
    public bool HasDeliveryOrders()
    
    // Order Item Information
    public bool IsOrderIdDisplayed(int index = 0)
    public bool IsOrderDateDisplayed(int index = 0)
    public bool IsDeliveryStatusDisplayed(int index = 0)
    public string GetOrderId(int index = 0)
    public string GetOrderDate(int index = 0)
    public string GetDeliveryStatus(int index = 0)
    
    // Loading and Error States
    public bool IsLoadingIndicatorDisplayed()
    public bool IsEmptyStateDisplayed()
    public string GetEmptyStateMessage()
}
```

**Locator Strategy**:
- Sử dụng CSS Selectors và XPath linh hoạt để tương thích với cấu trúc HTML động
- Ưu tiên data attributes hoặc class names ổn định
- Fallback sang text-based locators khi cần thiết

### 2. OrderTests (Test Class)

**Responsibility**: Chứa test case TC_F4_1_01 và các test cases liên quan đến Orders.

**Key Structure**:

```csharp
[TestFixture]
public class OrderTests : BaseTest
{
    private const string TestEmail = "shopclothing1525@gmail.com";
    private const string TestPassword = "Test@123";
    
    private void LoginAsTestUser()
    
    [Test]
    public void TC_F4_1_01()
}
```

### 3. Integration with Existing Components

**LoginPage**: Sử dụng để đăng nhập trước khi test (precondition)

**BasePage**: OrdersPage kế thừa để sử dụng các helper methods:
- `WaitForElementVisible()`
- `WaitForElementClickable()`
- `Click()`
- `GetText()`
- `IsDisplayed()`

**BaseTest**: OrderTests kế thừa để sử dụng:
- Setup/TearDown lifecycle
- Driver initialization
- Screenshot on failure
- Implicit wait configuration

## Data Models

### Order Item Display Model

Mỗi order item trong danh sách đang giao cần hiển thị:

```
OrderItem {
    OrderId: string          // Mã đơn hàng (VD: "#12345")
    OrderDate: string        // Ngày đặt hàng (VD: "28/03/2026")
    DeliveryStatus: string   // Trạng thái giao hàng (VD: "Đang giao")
    [Optional] TotalAmount: string
    [Optional] ItemCount: int
}
```

### Test Data

Test sử dụng tài khoản có sẵn:
- Email: `shopclothing1525@gmail.com`
- Password: `Test@123`

**Assumption**: Tài khoản này đã có ít nhất một đơn hàng với trạng thái "đang giao" trong hệ thống.

**Alternative**: Nếu không có đơn hàng đang giao, test cần verify empty state message.

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*


### Property 1: Orders Page Navigation

*For any* authenticated user navigating to the Orders page, the system should successfully display the Orders page.

**Validates: Requirements 1.1**

### Property 2: Authentication Guard

*For any* unauthenticated user attempting to access the Orders page, the system should redirect to the login page.

**Validates: Requirements 1.3**

### Property 3: Delivery Status Filtering

*For any* authenticated user with orders, when selecting the delivery list section, the system should display all and only orders belonging to that user with delivery status equal to "đang giao".

**Validates: Requirements 2.1, 3.1, 3.2**

### Property 4: Order Item Information Completeness

*For any* order item displayed in the delivery list, the system should display order identification, order date, and delivery status information.

**Validates: Requirements 2.2, 2.3, 2.4**

### Property 5: Consistent Order Sorting

*For any* list of delivery orders displayed, the system should present them in a consistent order (by date or order ID).

**Validates: Requirements 3.3**

### Property 6: Data Freshness on Refresh

*For any* page refresh action, the system should display the updated list of orders reflecting the current delivery status from the server.

**Validates: Requirements 3.4**

### Property 7: Navigation Options Availability

*For any* state where the user is viewing the Orders page or delivery list section, the system should display navigation options for different order status sections.

**Validates: Requirements 1.2, 4.3**

### Property 8: Order Detail Navigation

*For any* order item in the delivery list, when clicked, the system should display detailed information for that order and provide a way to return to the delivery list section.

**Validates: Requirements 4.1, 4.2**

### Property 9: Loading State Management

*For any* order list loading operation, the system should display a loading indicator during the load and hide it upon completion.

**Validates: Requirements 5.1, 5.2**

### Property 10: Error Recovery Option

*For any* error state encountered while loading orders, the system should provide a retry option to reload the order list.

**Validates: Requirements 6.3**

## Error Handling

### Error Scenarios

1. **Authentication Failure**
   - Scenario: User không đăng nhập hoặc session hết hạn
   - Handling: Redirect đến trang login
   - Test Verification: Kiểm tra URL chứa "/login"

2. **Empty State**
   - Scenario: Không có đơn hàng nào đang giao
   - Handling: Hiển thị thông báo empty state
   - Test Verification: Kiểm tra message "Không có đơn hàng đang giao" hoặc tương tự

3. **Network Error**
   - Scenario: Mất kết nối mạng khi load danh sách
   - Handling: Hiển thị thông báo lỗi mạng và nút retry
   - Test Verification: Kiểm tra error message và retry button

4. **Server Error**
   - Scenario: Server trả về lỗi 500 hoặc lỗi khác
   - Handling: Hiển thị thông báo lỗi phù hợp và nút retry
   - Test Verification: Kiểm tra error message và retry button

5. **Timeout**
   - Scenario: Load danh sách quá 15 giây
   - Handling: Hiển thị thông báo timeout
   - Test Verification: Kiểm tra timeout message sau 15s

### Error Handling Strategy in Tests

```csharp
// Sử dụng WebDriverWait với timeout phù hợp
private readonly WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));

// Kiểm tra cả trường hợp thành công và thất bại
Assert.Multiple(() => {
    if (ordersPage.HasDeliveryOrders()) {
        // Verify order list display
    } else {
        // Verify empty state message
    }
});
```

## Testing Strategy

### Dual Testing Approach

Test case này sử dụng kết hợp hai phương pháp:

1. **Unit Tests (Example-based)**: 
   - Test case TC_F4_1_01 là một example-based test
   - Kiểm tra một kịch bản cụ thể với dữ liệu test cụ thể
   - Verify các điểm kiểm tra cụ thể trong requirements

2. **Property-Based Tests** (Future Enhancement):
   - Có thể mở rộng với property-based testing cho các properties đã định nghĩa
   - Sử dụng thư viện như FsCheck for C#
   - Minimum 100 iterations per property test
   - Tag format: **Feature: order-delivery-list-view, Property {number}: {property_text}**

### Test Scope for TC_F4_1_01

Test case này tập trung vào **happy path scenario**:

**Preconditions**:
- User đã đăng nhập thành công
- Hệ thống có ít nhất một đơn hàng với trạng thái "đang giao" cho user này

**Test Steps**:
1. Login với test account
2. Navigate đến Orders page
3. Select "orders being delivered" section
4. Verify danh sách hiển thị đúng

**Assertions**:
- Orders page được hiển thị
- Delivery section được chọn và hiển thị
- Có ít nhất một order trong danh sách
- Mỗi order hiển thị đầy đủ: Order ID, Date, Status
- Status của các orders là "đang giao"

### Test Data Requirements

**Test Account**:
- Email: `shopclothing1525@gmail.com`
- Password: `Test@123`

**Data Assumptions**:
- Account này có ít nhất 1 đơn hàng đang giao
- Nếu không có, test sẽ verify empty state message thay vì order list

### Test Execution

**Single Execution Policy**: 
Theo yêu cầu của user, test chỉ được chạy một lần, không lặp lại nhiều lần. Test được thiết kế để:
- Chạy một lần và verify tất cả assertions
- Sử dụng `Assert.Multiple()` để thu thập tất cả failures
- Capture screenshot nếu fail (thông qua BaseTest.TearDown)

**Test Independence**:
- Test không phụ thuộc vào thứ tự chạy
- Test không tạo hoặc xóa dữ liệu (chỉ đọc)
- Test sử dụng dữ liệu có sẵn trong hệ thống

### Page Object Implementation Strategy

**Locator Flexibility**:
Do cấu trúc HTML có thể thay đổi, OrdersPage sẽ sử dụng multiple fallback locators:

```csharp
private IWebElement FindDeliverySection()
{
    // Try multiple strategies
    var strategies = new List<By> {
        By.CssSelector("[data-status='delivering']"),
        By.XPath("//a[contains(text(),'Đang giao')]"),
        By.XPath("//button[contains(text(),'Đang giao')]"),
        By.CssSelector(".order-status-tab.delivering")
    };
    
    foreach (var strategy in strategies) {
        var elements = driver.FindElements(strategy);
        if (elements.Any(e => e.Displayed)) {
            return elements.First(e => e.Displayed);
        }
    }
    throw new NoSuchElementException("Delivery section not found");
}
```

**Wait Strategy**:
- Sử dụng explicit waits (WebDriverWait) thay vì Thread.Sleep
- Wait cho elements visible và clickable
- Timeout mặc định: 15 seconds (từ BasePage)

**Verification Methods**:
- Trả về boolean cho các kiểm tra tồn tại/hiển thị
- Trả về string cho các giá trị text
- Trả về int cho các đếm số lượng
- Throw exception nếu element bắt buộc không tìm thấy

### Test Maintenance Considerations

**Resilience to UI Changes**:
- Sử dụng multiple locator strategies
- Prefer data attributes và stable class names
- Fallback sang text-based locators
- Avoid brittle XPath với nhiều levels

**Debugging Support**:
- Console.WriteLine() để log các bước quan trọng
- Screenshot tự động khi test fail
- Log current URL và page state

**Code Reusability**:
- LoginAsTestUser() helper method
- OrdersPage có thể tái sử dụng cho các test khác
- Tuân theo DRY principle

## Implementation Notes

### File Structure

```
TestProject1/
├── Pages/
│   └── OrdersPage.cs          (NEW - Page Object cho Orders page)
├── Tests/
│   └── OrderTests.cs          (NEW - Test class chứa TC_F4_1_01)
└── .kiro/specs/order-delivery-list-view/
    ├── .config.kiro
    ├── requirements.md
    └── design.md              (THIS FILE)
```

### Implementation Order

1. **Create OrdersPage.cs**
   - Implement basic navigation
   - Implement section selection
   - Implement verification methods
   - Add locators với fallback strategies

2. **Create OrderTests.cs**
   - Setup test class structure
   - Implement LoginAsTestUser() helper
   - Implement TC_F4_1_01 test method
   - Add assertions với Assert.Multiple()

3. **Test and Refine**
   - Run test một lần
   - Verify tất cả assertions
   - Adjust locators nếu cần
   - Ensure screenshot capture works

### Code Style Guidelines

- Tuân theo existing code style trong dự án
- Sử dụng primary constructor syntax: `public OrdersPage(IWebDriver driver) : base(driver)`
- Constants cho URLs và test data
- Descriptive method names
- XML comments cho public methods
- Console logging cho debugging

### Dependencies on Existing Code

**Must Use**:
- `BaseTest` - cho test lifecycle
- `BasePage` - cho page object base functionality
- `LoginPage` - cho authentication precondition
- `DriverFactory` - cho driver initialization (thông qua BaseTest)

**Must Not Modify**:
- Không thay đổi existing page objects
- Không thay đổi BaseTest hoặc BasePage
- Chỉ thêm code mới

## Assumptions and Constraints

### Assumptions

1. Test account `shopclothing1525@gmail.com` tồn tại và có thể login
2. Account này có ít nhất một đơn hàng với status "đang giao"
3. Orders page có URL pattern `/orders` hoặc `/account/orders`
4. Delivery section có thể được identify bằng text "Đang giao" hoặc data attribute
5. Order items hiển thị trong table hoặc list structure
6. Mỗi order item có thể identify được ID, date, và status

### Constraints

1. **Single Execution**: Test chỉ chạy một lần, không lặp lại
2. **Read-Only**: Test không tạo hoặc modify dữ liệu
3. **No External Dependencies**: Test không phụ thuộc vào external services ngoài web app
4. **Browser Compatibility**: Test chạy trên Chrome (theo DriverFactory config)
5. **Timeout Limits**: Maximum wait time là 15 seconds
6. **No Parallel Execution**: Test không được thiết kế cho parallel execution

### Known Limitations

1. Test phụ thuộc vào dữ liệu có sẵn trong hệ thống
2. Không test được các edge cases như network errors (cần mock/stub)
3. Không verify chính xác nội dung của order details (chỉ verify hiển thị)
4. Không test performance hoặc load time cụ thể
5. Locators có thể cần adjust nếu UI thay đổi đáng kể

## Future Enhancements

1. **Property-Based Testing**: Implement các properties đã định nghĩa với FsCheck
2. **Data-Driven Testing**: Parameterize test với nhiều test accounts
3. **Negative Test Cases**: Test authentication failures, empty states, errors
4. **Cross-Browser Testing**: Extend để chạy trên Firefox, Edge
5. **API Integration**: Verify data consistency giữa UI và API
6. **Performance Testing**: Measure và assert load times
7. **Accessibility Testing**: Verify WCAG compliance của Orders page

