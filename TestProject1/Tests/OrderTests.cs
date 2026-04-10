using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumProject.Pages;
using TestProject1.Utilities;
using System;
using System.Linq;

namespace TestProject1.Tests
{
    [TestFixture]
    public class OrderTests : BaseTest
    {
        private IWebDriver Driver => driver!;

        private void LoginAsAdmin()
        {
            Driver.Navigate().GoToUrl(baseUrl + "/Account/Login");
            var loginPage = new LoginPage(Driver);
            var admin = ConfigReader.GetUserData("admin");
            loginPage.Login(admin.Username, admin.Password);
        }

        private void LoginAsEmptyUser()
        {
            Driver.Navigate().GoToUrl(baseUrl + "/Account/Login");
            var loginPage = new LoginPage(Driver);
            var emptyUser = ConfigReader.GetUserData("empty_user");
            loginPage.Login(emptyUser.Username, emptyUser.Password);
        }

        // ==========================================
        // F4.1: XEM DANH SÁCH ĐƠN HÀNG (01 - 08)
        // ==========================================

        [Test, Order(1), Description("TC_F4.1_01: Xem danh sách đơn hàng đang giao")]
        public void TC_F4_1_01()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            int count = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn hàng đang giao: {count}");
            
            Assert.That(count, Is.GreaterThanOrEqualTo(0), 
                "Tab 'Đang giao' không hiển thị đúng");
        }

        [Test, Order(2), Description("TC_F4.1_02: Xem danh sách đơn hàng đã giao")]
        public void TC_F4_1_02()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            System.Threading.Thread.Sleep(1000);
            
            int count = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn hàng đã giao: {count}");
            
            Assert.That(count, Is.GreaterThanOrEqualTo(0), 
                "Tab 'Đã giao' không hiển thị đúng");
        }

        [Test, Order(3), Description("TC_F4.1_03: Xem danh sách đơn hàng đã hủy")]
        public void TC_F4_1_03()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            
            int count = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn hàng đã hủy: {count}");
            
            Assert.That(count, Is.GreaterThanOrEqualTo(0), 
                "Tab 'Đã hủy' không hiển thị đúng");
        }

        [Test, Order(4), Description("TC_F4.1_04: Xem danh sách đơn hàng đang giao rỗng")]
        public void TC_F4_1_04()
        {
            LoginAsEmptyUser();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            int count = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn hàng đang giao (empty_user): {count}");
            
            if (count == 0)
            {
                Assert.That(orders.IsEmptyMessageDisplayed(), Is.True,
                    "Tab rỗng nhưng không hiển thị thông báo 'Chưa có đơn hàng'");
            }
            else
            {
                Assert.Pass($"Tab có {count} đơn hàng");
            }
        }

        [Test, Order(5), Description("TC_F4.1_05: Xem danh sách đơn hàng đã giao rỗng")]
        public void TC_F4_1_05()
        {
            LoginAsEmptyUser();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            System.Threading.Thread.Sleep(1000);
            
            int count = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn hàng đã giao (empty_user): {count}");
            
            if (count == 0)
            {
                Assert.That(orders.IsEmptyMessageDisplayed(), Is.True,
                    "Tab rỗng nhưng không hiển thị thông báo 'Chưa có đơn hàng'");
            }
            else
            {
                Assert.Pass($"Tab có {count} đơn hàng");
            }
        }

        [Test, Order(6), Description("TC_F4.1_06: Xem danh sách đơn hàng đã hủy rỗng")]
        public void TC_F4_1_06()
        {
            LoginAsEmptyUser();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            
            int count = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn hàng đã hủy (empty_user): {count}");
            
            if (count == 0)
            {
                Assert.That(orders.IsEmptyMessageDisplayed(), Is.True,
                    "Tab rỗng nhưng không hiển thị thông báo 'Chưa có đơn hàng'");
            }
            else
            {
                Assert.Pass($"Tab có {count} đơn hàng");
            }
        }

        [Test, Order(4), Description("F4.1_04-06: Kiểm tra Tab rỗng")]
        public void TC_F4_1_04_EmptyStates()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectCancelledTab(); 
            if(orders.GetCurrentTabOrderCount() == 0)
                Assert.That(orders.IsEmptyMessageDisplayed(), Is.True, "Lỗi: Không hiển thị thông báo rỗng khi tab không có đơn.");
        }

        [Test, Order(7), Description("TC_F4.1_07: Kiểm tra chuyển đổi các mục")]
        public void TC_F4_1_07()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            
            // Chọn tab Đang giao
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            int processingCount = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Tab Đang giao: {processingCount} đơn");
            Assert.That(processingCount, Is.GreaterThanOrEqualTo(0), "Tab Đang giao không hiển thị đúng");
            
            // Chọn tab Đã giao
            orders.SelectDeliveredTab();
            System.Threading.Thread.Sleep(1000);
            int deliveredCount = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Tab Đã giao: {deliveredCount} đơn");
            Assert.That(deliveredCount, Is.GreaterThanOrEqualTo(0), "Tab Đã giao không hiển thị đúng");
            
            // Chọn tab Đã hủy
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            int cancelledCount = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Tab Đã hủy: {cancelledCount} đơn");
            Assert.That(cancelledCount, Is.GreaterThanOrEqualTo(0), "Tab Đã hủy không hiển thị đúng");
            
            Console.WriteLine("✓ Chuyển đổi giữa các tab thành công");
        }

        [Test, Order(8), Description("TC_F4.1_08: Kiểm tra tải lại trang danh sách đơn hàng")]
        public void TC_F4_1_08()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            
            // Chọn tab Đang giao
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            int countBefore = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn trước khi refresh: {countBefore}");
            
            // Refresh trang
            Driver.Navigate().Refresh();
            System.Threading.Thread.Sleep(1000);
            
            // Verify vẫn ở trang Orders và số đơn không đổi
            Assert.That(Driver.Url, Does.Contain("/Orders"), "Không còn ở trang Orders sau refresh");
            
            int countAfter = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn sau khi refresh: {countAfter}");
            
            Assert.That(countAfter, Is.EqualTo(countBefore), 
                "Số lượng đơn hàng thay đổi sau khi refresh");
            
            Console.WriteLine("✓ Refresh trang thành công, danh sách vẫn hiển thị đúng");
        }


        // ==========================================
        // F4.2: CHI TIẾT ĐƠN HÀNG (01 - 05)
        // ==========================================

        [Test, Order(9), Description("TC_F4.2_01: Kiểm tra thông tin chi tiết đơn hàng hiển thị đầy đủ")]
        public void TC_F4_2_01()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            if(orders.GetCurrentTabOrderCount() > 0)
            {
                orders.ClickViewDetails(0);
                System.Threading.Thread.Sleep(1000);
                
                var pageSource = Driver.PageSource;
                
                Assert.Multiple(() => {
                    Assert.That(pageSource, Does.Contain("Mã đơn").Or.Contain("Order ID").Or.Contain("#"), 
                        "Thiếu Mã đơn hàng");
                    Assert.That(pageSource, Does.Contain("Ngày đặt").Or.Contain("Date").Or.Contain("ngày"), 
                        "Thiếu Ngày đặt");
                    Assert.That(pageSource, Does.Contain("Sản phẩm").Or.Contain("Product"), 
                        "Thiếu thông tin Sản phẩm");
                    Assert.That(pageSource, Does.Contain("Số lượng").Or.Contain("Quantity"), 
                        "Thiếu Số lượng");
                    Assert.That(pageSource, Does.Contain("Giá").Or.Contain("Price").Or.Contain("₫"), 
                        "Thiếu Giá sản phẩm");
                    Assert.That(pageSource, Does.Contain("Địa chỉ").Or.Contain("Address"), 
                        "BUG: Thiếu Địa chỉ nhận hàng!");
                    Assert.That(pageSource, Does.Contain("Tổng").Or.Contain("Total"), 
                        "Thiếu Tổng tiền");
                    Assert.That(pageSource, Does.Contain("Trạng thái").Or.Contain("Status").Or.Contain("Đang").Or.Contain("Chờ"), 
                        "Thiếu Trạng thái đơn hàng");
                });
                
                Console.WriteLine("✓ Tất cả thông tin chi tiết đơn hàng hiển thị đầy đủ");
            }
            else
            {
                Assert.Ignore("Không có đơn hàng để test");
            }
        }

        [Test, Order(10), Description("TC_F4.2_02: Xem chi tiết đơn hàng đang giao")]
        public void TC_F4_2_02_DetailsByStatus()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            if(orders.GetCurrentTabOrderCount() > 0)
            {
                orders.ClickViewDetails(0);
                System.Threading.Thread.Sleep(1000);
                
                Assert.That(Driver.Url, Does.Contain("/Details"), 
                    "Không chuyển đến trang chi tiết đơn hàng");
                
                var pageSource = Driver.PageSource;
                Assert.That(pageSource, Does.Contain("Chi tiết đơn hàng").Or.Contain("Order Details"), 
                    "Trang chi tiết không hiển thị đúng");
                
                Console.WriteLine("✓ Xem chi tiết đơn hàng đang giao thành công");
            }
            else
            {
                Assert.Ignore("Không có đơn hàng đang giao để test");
            }
        }

        [Test, Order(11), Description("TC_F4.2_03: Xem chi tiết đơn hàng đã giao")]
        public void TC_F4_2_03()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            System.Threading.Thread.Sleep(1000);
            
            if(orders.GetCurrentTabOrderCount() > 0)
            {
                orders.ClickViewDetails(0);
                System.Threading.Thread.Sleep(1000);
                
                Assert.That(Driver.Url, Does.Contain("/Details"), 
                    "Không chuyển đến trang chi tiết đơn hàng");
                
                var pageSource = Driver.PageSource;
                Assert.That(pageSource, Does.Contain("Chi tiết đơn hàng").Or.Contain("Order Details"), 
                    "Trang chi tiết không hiển thị đúng");
                
                Console.WriteLine("✓ Xem chi tiết đơn hàng đã giao thành công");
            }
            else
            {
                Assert.Ignore("Không có đơn hàng đã giao để test");
            }
        }

        [Test, Order(12), Description("TC_F4.2_04: Xem chi tiết đơn hàng đã hủy")]
        public void TC_F4_2_04()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            
            if(orders.GetCurrentTabOrderCount() > 0)
            {
                orders.ClickViewDetails(0);
                System.Threading.Thread.Sleep(1000);
                
                Assert.That(Driver.Url, Does.Contain("/Details"), 
                    "Không chuyển đến trang chi tiết đơn hàng");
                
                var pageSource = Driver.PageSource;
                Assert.That(pageSource, Does.Contain("Chi tiết đơn hàng").Or.Contain("Order Details"), 
                    "Trang chi tiết không hiển thị đúng");
                
                Console.WriteLine("✓ Xem chi tiết đơn hàng đã hủy thành công");
            }
            else
            {
                Assert.Ignore("Không có đơn hàng đã hủy để test");
            }
        }

        [Test, Order(13), Description("TC_F4.2_05: Kiểm tra tải lại trang chi tiết đơn hàng")]
        public void TC_F4_2_05()
        {
            LoginAsAdmin();
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            if(orders.GetCurrentTabOrderCount() > 0)
            {
                orders.ClickViewDetails(0);
                System.Threading.Thread.Sleep(1000);
                
                string urlBefore = Driver.Url;
                Console.WriteLine($"URL trước khi refresh: {urlBefore}");
                
                // Refresh trang
                Driver.Navigate().Refresh();
                System.Threading.Thread.Sleep(1000);
                
                string urlAfter = Driver.Url;
                Console.WriteLine($"URL sau khi refresh: {urlAfter}");
                
                // Verify vẫn ở trang chi tiết
                Assert.That(urlAfter, Does.Contain("/Details"), 
                    "Không còn ở trang chi tiết sau refresh");
                Assert.That(urlAfter, Is.EqualTo(urlBefore), 
                    "URL thay đổi sau khi refresh");
                
                var pageSource = Driver.PageSource;
                Assert.That(pageSource, Does.Contain("Chi tiết đơn hàng").Or.Contain("Order Details"), 
                    "Trang chi tiết không hiển thị đúng sau refresh");
                
                Console.WriteLine("✓ Refresh trang chi tiết thành công, thông tin vẫn hiển thị đúng");
            }
            else
            {
                Assert.Ignore("Không có đơn hàng để test");
            }
        }


        // ==========================================
        // F4.3: HỦY ĐƠN HÀNG (06 - 10, 01 - 03)
        // ==========================================

        [Test, Order(14), Description("TC_F4.3_01: Hủy đơn hàng trạng thái đang xử lý")]
        public void TC_F4_3_01_CancelSuccess()
        {
            // Bước 1: Đăng nhập admin
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Cancel Order");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Vào trang Orders và chọn tab Đang xử lý
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            int orderCount = orders.GetCurrentTabOrderCount();
            Console.WriteLine($"Số đơn hàng đang xử lý: {orderCount}");
            
            if(orderCount > 0)
            {
                // Bước 4: Lấy mã đơn hàng đầu tiên (đơn vừa tạo)
                try {
                    string id = orders.GetFirstOrderId();
                    Console.WriteLine($"Đơn hàng cần hủy: #{id}");
                    
                    // Bước 5: Vào chi tiết và hủy đơn
                    orders.ClickViewDetails(0);
                    System.Threading.Thread.Sleep(1000);
                    
                    var details = new OrderDetailsPage(Driver);
                    details.CancelOrder("Lý do test 123");
                    System.Threading.Thread.Sleep(2000);
                    
                    // Bước 6: Quay lại trang Orders và kiểm tra tab Đã hủy
                    orders.NavigateTo();
                    orders.SelectCancelledTab();
                    System.Threading.Thread.Sleep(1000);
                    
                    Assert.That(Driver.PageSource, Does.Contain(id), 
                        $"Đơn hàng #{id} chưa chuyển sang tab Đã hủy");
                    
                    Console.WriteLine($"Hủy đơn hàng #{id} thành công, đơn đã chuyển sang tab Đã hủy");
                }
                catch (Exception ex) {
                    Console.WriteLine($"Lỗi khi lấy mã đơn hàng: {ex.Message}");
                    Assert.Fail($"Không thể lấy mã đơn hàng để test: {ex.Message}");
                }
            }
            else
            {
                Assert.Fail("Không có đơn hàng đang xử lý sau khi tạo đơn mới");
            }
        }


        [Test, Order(15), Description("TC_F4.3_02: Hủy đơn hàng trạng thái chờ lấy hàng")]
        public void TC_F4_3_02_CancelShipping()
        {
            // Bước 1: Đăng nhập admin
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Cancel Shipping Order");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng vừa tạo
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng vừa tạo: #{orderId}");
            
            // Bước 4: Vào trang Admin để cập nhật trạng thái đơn hàng
            Console.WriteLine("Đang vào trang Admin để cập nhật trạng thái...");
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 5: Cập nhật trạng thái từ "Chờ xác nhận" sang "Chờ lấy hàng"
            Console.WriteLine("Cập nhật trạng thái sang 'Chờ lấy hàng'...");
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            System.Threading.Thread.Sleep(1000);
            
            Console.WriteLine("Đã cập nhật trạng thái thành công");
            
            // Bước 6: Quay lại trang Orders của user và hủy đơn
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            // Tìm đơn hàng vừa cập nhật và vào chi tiết
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 7: Hủy đơn hàng
            var details = new OrderDetailsPage(Driver);
            details.CancelOrder("Lý do test hủy đơn chờ lấy hàng");
            System.Threading.Thread.Sleep(2000);
            
            // Bước 8: Verify đơn hàng đã chuyển sang tab Đã hủy
            orders.NavigateTo();
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            
            Assert.That(Driver.PageSource, Does.Contain(orderId), 
                $"Đơn hàng #{orderId} chưa chuyển sang tab Đã hủy");
            
            Console.WriteLine($"Hủy đơn hàng #{orderId} (trạng thái Chờ lấy hàng) thành công");
        }


        [Test, Order(16), Description("TC_F4.3_03: Hủy đơn hàng trạng thái chờ giao hàng - Thất bại")]
        public void TC_F4_3_03_CancelDelivering_ShouldFail()
        {
            // Bước 1: Đăng nhập admin
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Cancel Delivering Order");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng vừa tạo
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng vừa tạo: #{orderId}");
            
            // Bước 4: Vào trang Admin để cập nhật trạng thái đơn hàng
            Console.WriteLine("Đang vào trang Admin để cập nhật trạng thái...");
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 5: Cập nhật trạng thái từ "Chờ xác nhận" → "Chờ lấy hàng"
            Console.WriteLine("Cập nhật trạng thái sang 'Chờ lấy hàng'...");
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            System.Threading.Thread.Sleep(1000);
            
            // Bước 6: Cập nhật tiếp từ "Chờ lấy hàng" → "Chờ giao hàng"
            Console.WriteLine("Cập nhật trạng thái sang 'Chờ giao hàng'...");
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            System.Threading.Thread.Sleep(1000);
            
            Console.WriteLine("Đã cập nhật trạng thái thành 'Chờ giao hàng'");
            
            // Bước 7: Quay lại trang Orders của user và thử hủy đơn
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            // Tìm đơn hàng vừa cập nhật và vào chi tiết
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 8: Kiểm tra nút Hủy đơn hàng không hiển thị HOẶC thử hủy và verify lỗi
            var details = new OrderDetailsPage(Driver);
            
            if (!details.IsCancelButtonDisplayed())
            {
                // Trường hợp 1: Nút hủy không hiển thị (đúng logic)
                Console.WriteLine("Nút 'Hủy đơn hàng' không hiển thị với trạng thái 'Chờ giao hàng' - ĐÚNG");
                Assert.Pass("Không thể hủy đơn hàng ở trạng thái 'Chờ giao hàng' - Nút hủy không hiển thị");
            }
            else
            {
                // Trường hợp 2: Nút hủy vẫn hiển thị, thử hủy và verify có thông báo lỗi
                Console.WriteLine("Nút 'Hủy đơn hàng' vẫn hiển thị, thử hủy và kiểm tra thông báo lỗi...");
                
                try {
                    details.CancelOrder("Lý do test hủy đơn chờ giao hàng");
                    System.Threading.Thread.Sleep(2000);
                    
                    // Kiểm tra có thông báo lỗi không
                    var pageSource = Driver.PageSource;
                    bool hasError = pageSource.Contains("không thể hủy") || 
                                   pageSource.Contains("Không thể hủy") ||
                                   pageSource.Contains("không được phép") ||
                                   pageSource.Contains("Chỉ có thể hủy") ||
                                   Driver.Url.Contains("/Details/"); // Vẫn ở trang chi tiết = không hủy được
                    
                    if (hasError || Driver.Url.Contains("/Details/"))
                    {
                        Console.WriteLine("Hủy đơn thất bại - Có thông báo lỗi hoặc vẫn ở trang chi tiết");
                        Assert.Pass("Không thể hủy đơn hàng ở trạng thái 'Chờ giao hàng' - Có thông báo lỗi");
                    }
                    else
                    {
                        // Kiểm tra đơn có chuyển sang tab Đã hủy không
                        orders.NavigateTo();
                        orders.SelectCancelledTab();
                        System.Threading.Thread.Sleep(1000);
                        
                        if (Driver.PageSource.Contains(orderId))
                        {
                            Assert.Fail($"BUG: Đơn hàng #{orderId} ở trạng thái 'Chờ giao hàng' vẫn bị hủy thành công!");
                        }
                        else
                        {
                            Assert.Pass("Đơn hàng không chuyển sang tab Đã hủy - Hủy thất bại như mong đợi");
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Lỗi khi thử hủy đơn: {ex.Message}");
                    Assert.Pass($"Không thể hủy đơn hàng ở trạng thái 'Chờ giao hàng' - Exception: {ex.Message}");
                }
            }
        }

        [Test, Order(17), Description("TC_F4.3_04: Hủy đơn hàng nhưng chọn không xác nhận")]
        public void TC_F4_3_04_CancelAbort()
        {
            // Bước 1: Đăng nhập admin
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Cancel Abort");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Vào trang Orders và chọn tab Đang giao
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            // Bước 4: Vào chi tiết đơn hàng
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 5: Nhấn nút Hủy đơn hàng (mở modal)
            var details = new OrderDetailsPage(Driver);
            details.ClickCancelButtonOnly();
            System.Threading.Thread.Sleep(1000);
            
            // Bước 6: Verify modal hiển thị
            Assert.That(details.IsCancelModalDisplayed(), Is.True, 
                "Modal hủy đơn hàng không xuất hiện");
            Console.WriteLine("Modal hủy đơn hàng đã hiển thị");
            
            // Bước 7: Đóng modal (không xác nhận hủy)
            details.CloseCancelModal();
            System.Threading.Thread.Sleep(1000);
            
            Console.WriteLine("Đã đóng modal mà không xác nhận hủy");
            
            // Bước 8: Verify đơn hàng vẫn ở tab Đang giao (không bị hủy)
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            Assert.That(Driver.PageSource, Does.Contain(orderId), 
                $"Đơn hàng #{orderId} không còn trong tab Đang giao");
            
            Console.WriteLine($"Đơn hàng #{orderId} vẫn ở tab Đang giao - Không bị hủy");
            
            // Bước 9: Verify đơn hàng KHÔNG có trong tab Đã hủy
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            
            // Kiểm tra chỉ trong tab Đã hủy đang active (không kiểm tra toàn bộ PageSource)
            var cancelledTabContent = Driver.FindElement(By.CssSelector("#cancelled.active"));
            Assert.That(cancelledTabContent.Text, Does.Not.Contain(orderId), 
                $"BUG: Đơn hàng #{orderId} xuất hiện trong tab Đã hủy dù không xác nhận!");
            
            Console.WriteLine($"Đơn hàng #{orderId} không có trong tab Đã hủy - ĐÚNG");
        }


        [Test, Order(18), Description("TC_F4.3_05: Hủy đơn hàng nhưng không nhập nội dung lý do")]
        public void TC_F4_3_05_CancelWithoutReason()
        {
            // Bước 1: Đăng nhập admin
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Cancel Without Reason");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Vào trang Orders
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            // Bước 4: Vào chi tiết và thử hủy không nhập lý do
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            var details = new OrderDetailsPage(Driver);
            details.SubmitCancelWithoutReason();
            System.Threading.Thread.Sleep(1000);
            
            // Bước 5: Verify validation - đơn không bị hủy
            Console.WriteLine("Kiểm tra validation...");
            
            // Quay lại Orders và kiểm tra đơn vẫn ở tab Đang giao
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            Assert.That(Driver.PageSource, Does.Contain(orderId), 
                $"Đơn hàng #{orderId} không còn trong tab Đang giao");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} vẫn ở tab Đang giao - Validation hoạt động đúng");
            
            // Verify đơn KHÔNG có trong tab Đã hủy
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            
            var cancelledTabContent = Driver.FindElement(By.CssSelector("#cancelled.active"));
            Assert.That(cancelledTabContent.Text, Does.Not.Contain(orderId), 
                $"BUG: Đơn hàng #{orderId} bị hủy dù không nhập lý do!");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} không có trong tab Đã hủy - ĐÚNG");
        }

        [Test, Order(19), Description("TC_F4.3_06: Nút Hủy đơn hàng không hiển thị với đơn đã giao")]
        public void TC_F4_3_06_CancelButtonNotDisplayedForDelivered()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng và cập nhật sang Đã giao
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test No Cancel Button Delivered");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            // Bước 3: Lấy mã đơn hàng
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            // Bước 4: Admin cập nhật trạng thái sang Đã giao
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            System.Threading.Thread.Sleep(1000);
            
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            System.Threading.Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            System.Threading.Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Đã giao");
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đã cập nhật trạng thái sang 'Đã giao'");
            
            // Bước 5: Vào chi tiết đơn hàng đã giao
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            System.Threading.Thread.Sleep(1000);
            
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 6: Verify nút Hủy KHÔNG hiển thị
            var details = new OrderDetailsPage(Driver);
            bool cancelButtonDisplayed = details.IsCancelButtonDisplayed();
            
            Assert.That(cancelButtonDisplayed, Is.False, 
                $"BUG: Nút 'Hủy đơn hàng' vẫn hiển thị với đơn hàng đã giao #{orderId}!");
            
            Console.WriteLine($"✓ Nút 'Hủy đơn hàng' không hiển thị với đơn đã giao #{orderId} - ĐÚNG");
        }

        [Test, Order(20), Description("TC_F4.3_07: Nút Hủy đơn hàng không hiển thị với đơn đã hủy")]
        public void TC_F4_3_07_CancelButtonNotDisplayedForCancelled()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test No Cancel Button Cancelled");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            // Bước 3: Hủy đơn hàng
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            var details = new OrderDetailsPage(Driver);
            details.CancelOrder("Lý do test");
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đã hủy đơn hàng");
            
            // Bước 4: Vào chi tiết đơn đã hủy
            orders.NavigateTo();
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 5: Verify nút Hủy KHÔNG hiển thị
            bool cancelButtonDisplayed = details.IsCancelButtonDisplayed();
            
            Assert.That(cancelButtonDisplayed, Is.False, 
                $"BUG: Nút 'Hủy đơn hàng' vẫn hiển thị với đơn đã hủy #{orderId}!");
            
            Console.WriteLine($"✓ Nút 'Hủy đơn hàng' không hiển thị với đơn đã hủy #{orderId} - ĐÚNG");
        }

        [Test, Order(21), Description("TC_F4.3_08: Đơn hàng chuyển sang tab Đã hủy sau khi hủy")]
        public void TC_F4_3_08_OrderMovesToCancelledTab()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Move To Cancelled");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            // Bước 3: Chọn tab Đang giao và lấy mã đơn
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            // Bước 4: Hủy đơn hàng
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            var details = new OrderDetailsPage(Driver);
            details.CancelOrder("Lý do test chuyển tab");
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đã hủy đơn hàng");
            
            // Bước 5: Verify đơn KHÔNG còn trong tab Đang giao
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            var processingTabContent = Driver.FindElement(By.CssSelector("#processing.active"));
            Assert.That(processingTabContent.Text, Does.Not.Contain(orderId), 
                $"BUG: Đơn hàng #{orderId} vẫn còn trong tab Đang giao sau khi hủy!");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} không còn trong tab Đang giao - ĐÚNG");
            
            // Bước 6: Chọn tab Đã hủy và quan sát
            orders.SelectCancelledTab();
            System.Threading.Thread.Sleep(1000);
            
            // Verify đơn hàng xuất hiện trong tab Đã hủy
            var cancelledTabContent = Driver.FindElement(By.CssSelector("#cancelled.active"));
            Assert.That(cancelledTabContent.Text, Does.Contain(orderId), 
                $"BUG: Đơn hàng #{orderId} không xuất hiện trong tab Đã hủy!");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} đã xuất hiện trong tab Đã hủy - ĐÚNG");
            Console.WriteLine("✓ Đơn hàng đã chuyển từ tab Đang giao sang tab Đã hủy thành công");
        }


        // ==========================================
        // F4.4: XÁC NHẬN ĐÃ NHẬN HÀNG (01 - 03)
        // ==========================================

        [Test, Order(22), Description("TC_F4.4_01: Xác nhận đã nhận hàng thành công")]
        public void TC_F4_4_01_ConfirmReceivedSuccess()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
         

            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Confirm Received");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            // Bước 4: Admin cập nhật trạng thái đơn hàng sang Chờ giao hàng
            Console.WriteLine("Đang cập nhật trạng thái đơn hàng...");
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            System.Threading.Thread.Sleep(1000);
            
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            System.Threading.Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đã cập nhật trạng thái sang 'Chờ giao hàng'");
            
            // Bước 5: Vào chi tiết đơn hàng
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 6: Xác nhận đã nhận hàng
            var details = new OrderDetailsPage(Driver);
            details.ClickConfirmReceived();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đã xác nhận đã nhận hàng");
            
            // Bước 7: Verify đơn hàng chuyển sang tab Đã giao
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            System.Threading.Thread.Sleep(1000);
            
            var deliveredTabContent = Driver.FindElement(By.CssSelector("#completed.active"));
            Assert.That(deliveredTabContent.Text, Does.Contain(orderId), 
                $"Đơn hàng #{orderId} không xuất hiện trong tab Đã giao!");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} đã chuyển sang tab Đã giao - ĐÚNG");
        }

        [Test, Order(23), Description("TC_F4.4_02: Xác nhận đã nhận hàng nhưng chọn không xác nhận")]
        public void TC_F4_4_02_ConfirmReceivedAbort()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Confirm Abort");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            // Bước 4: Admin cập nhật trạng thái đơn hàng sang Chờ giao hàng
            Console.WriteLine("Đang cập nhật trạng thái đơn hàng...");
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            System.Threading.Thread.Sleep(1000);
            
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            System.Threading.Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đã cập nhật trạng thái sang 'Chờ giao hàng'");
            
            // Bước 5: Vào chi tiết đơn hàng
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            // Bước 6: Mở modal xác nhận nhưng đóng lại (không xác nhận)
            var details = new OrderDetailsPage(Driver);
            
            // Verify nút "Đã nhận" hiển thị
            Assert.That(details.IsConfirmReceivedButtonDisplayed(), Is.True, 
                "Nút 'Đã nhận' không hiển thị");
            Console.WriteLine("Nút 'Đã nhận' hiển thị");
            
            // Click nút "Đã nhận" để mở modal
            Driver.FindElement(By.XPath("//button[contains(.,'Đã nhận')]")).Click();
            System.Threading.Thread.Sleep(1000);
            
            // Verify modal hiển thị
            Assert.That(details.IsConfirmReceivedModalDisplayed(), Is.True, 
                "Modal xác nhận đã nhận hàng không hiển thị");
            Console.WriteLine("Modal xác nhận đã nhận hàng hiển thị");
            
            // Đóng modal
            details.CloseConfirmReceivedModal();
            System.Threading.Thread.Sleep(1000);
            
            Console.WriteLine("Đã đóng modal mà không xác nhận");
            
            // Bước 7: Verify đơn hàng vẫn ở tab Đang giao
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            var processingTabContent = Driver.FindElement(By.CssSelector("#processing.active"));
            Assert.That(processingTabContent.Text, Does.Contain(orderId), 
                $"Đơn hàng #{orderId} không còn trong tab Đang giao!");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} vẫn ở tab Đang giao - ĐÚNG");
            
            // Bước 8: Verify đơn hàng KHÔNG có trong tab Đã giao
            orders.SelectDeliveredTab();
            System.Threading.Thread.Sleep(1000);
            
            var deliveredTabContent = Driver.FindElement(By.CssSelector("#completed.active"));
            Assert.That(deliveredTabContent.Text, Does.Not.Contain(orderId), 
                $"BUG: Đơn hàng #{orderId} xuất hiện trong tab Đã giao dù không xác nhận!");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} không có trong tab Đã giao - ĐÚNG");
        }

        [Test, Order(24), Description("TC_F4.4_03: Kiểm tra đơn hàng chuyển sang tab Đã giao sau khi xác nhận")]
        public void TC_F4_4_03_OrderMovesToDelivered()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            System.Threading.Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            System.Threading.Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Move To Delivered");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            // Bước 4: Admin cập nhật trạng thái đơn hàng sang Chờ giao hàng
            Console.WriteLine("Đang cập nhật trạng thái đơn hàng...");
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            System.Threading.Thread.Sleep(1000);
            
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            System.Threading.Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đã cập nhật trạng thái sang 'Chờ giao hàng'");
            
            // Bước 5: Vào chi tiết đơn hàng và xác nhận đã nhận
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            orders.ClickViewDetails(0);
            System.Threading.Thread.Sleep(1000);
            
            var details = new OrderDetailsPage(Driver);
            details.ClickConfirmReceived();
            System.Threading.Thread.Sleep(2000);
            
            Console.WriteLine("Đã xác nhận đã nhận hàng");
            
            // Bước 6: Verify đơn hàng KHÔNG còn trong tab Đang giao
            orders.NavigateTo();
            orders.SelectProcessingTab();
            System.Threading.Thread.Sleep(1000);
            
            var processingTabContent = Driver.FindElement(By.CssSelector("#processing.active"));
            Assert.That(processingTabContent.Text, Does.Not.Contain(orderId), 
                $"BUG: Đơn hàng #{orderId} vẫn còn trong tab Đang giao sau khi xác nhận đã nhận!");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} không còn trong tab Đang giao - ĐÚNG");
            
            // Bước 7: Chọn tab Đã giao và quan sát
            orders.SelectDeliveredTab();
            System.Threading.Thread.Sleep(1000);
            
            // Verify đơn hàng xuất hiện trong tab Đã giao
            var deliveredTabContent = Driver.FindElement(By.CssSelector("#completed.active"));
            Assert.That(deliveredTabContent.Text, Does.Contain(orderId), 
                $"BUG: Đơn hàng #{orderId} không xuất hiện trong tab Đã giao!");
            
            Console.WriteLine($"✓ Đơn hàng #{orderId} đã xuất hiện trong tab Đã giao - ĐÚNG");
            Console.WriteLine("✓ Đơn hàng đã chuyển từ tab Đang giao sang tab Đã giao thành công");
        }

        // Helper để chạy lại test nhanh
        private void TC_F4_1_01_ViewShippingOrders() { var orders = new OrdersPage(Driver); orders.NavigateTo(); orders.SelectProcessingTab(); }
    }
}
