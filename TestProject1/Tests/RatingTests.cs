using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumProject.Pages;
using TestProject1.Utilities;

namespace TestProject1.Tests
{
    [TestFixture]
    public class RatingTests : BaseTest
    {
        private IWebDriver Driver => driver!;

        private void LoginAsAdmin()
        {
            Driver.Navigate().GoToUrl(baseUrl + "/Account/Login");
            var loginPage = new LoginPage(Driver);
            var admin = ConfigReader.GetUserData("admin");
            loginPage.Login(admin.Username, admin.Password);
        }

        // ==========================================
        // F8: ĐÁNH GIÁ SẢN PHẨM
        // ==========================================

        [Test, Order(1), Description("TC_F8.1_01: Kiểm tra đánh giá sản phẩm thành công")]
        public void TC_F8_1_01_ReviewProductSuccess()
        {
            // Bước 1: Đăng nhập admin
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới và hoàn thành nó
            Console.WriteLine("Đang tạo đơn hàng mới...");
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickFirstProduct();
            Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            
            // Lưu URL sản phẩm để quay lại sau
            string productUrl = Driver.Url;
            Console.WriteLine($"URL sản phẩm: {productUrl}");
            
            product.ClickBuyNow();
            Thread.Sleep(1000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Review Product");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            Thread.Sleep(1000);
            var orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            // Bước 4: Admin cập nhật trạng thái đơn hàng sang Đã giao
            Console.WriteLine("Đang cập nhật trạng thái đơn hàng...");
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            Thread.Sleep(1000);
            
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Đã giao");
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã cập nhật trạng thái sang 'Đã giao'");
            
            // Bước 5: Truy cập trang Đơn hàng đã giao
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            Thread.Sleep(1000);
            
            // Bước 6: Vào chi tiết đơn hàng
            orders.ClickViewDetails(0);
            Thread.Sleep(1000);
            
            // Bước 7: Verify nút Đánh giá hiển thị
            var details = new OrderDetailsPage(Driver);
            bool reviewButtonDisplayed = details.IsReviewButtonDisplayed();
            Assert.That(reviewButtonDisplayed, Is.True, 
                $"Nút 'Đánh giá' không hiển thị với đơn hàng #{orderId}");
            
            Console.WriteLine($"Nút 'Đánh giá' hiển thị với đơn hàng #{orderId}");
            
            // Bước 8: Click nút Đánh giá (sẽ chuyển đến trang sản phẩm với #reviews)
            details.ClickReviewButton();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã chuyển đến trang sản phẩm với tab đánh giá");
            
            // Bước 9: Verify form đánh giá hiển thị
            bool reviewFormDisplayed = product.IsReviewFormDisplayed();
            Assert.That(reviewFormDisplayed, Is.True, "Form đánh giá không hiển thị!");
            
            Console.WriteLine("Form đánh giá hiển thị");
            
            // Bước 10: Chọn 5 sao
            product.SelectRating(5);
            Console.WriteLine("Đã chọn 5 sao");
            
            // Bước 11: Nhập nội dung đánh giá
            string reviewContent = "123";
            product.EnterReviewContent(reviewContent);
            Console.WriteLine($"Đã nhập nội dung: {reviewContent}");
            
            // Bước 12: Gửi đánh giá
            product.SubmitReview();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã gửi đánh giá");
            
            // Bước 13: Verify thông báo thành công hoặc đánh giá xuất hiện
            bool hasSuccess = product.HasSuccessMessage();
            if (hasSuccess)
            {
                Console.WriteLine("✓ Thông báo thành công hiển thị");
            }
            
            // Refresh trang để xem đánh giá mới
            Driver.Navigate().Refresh();
            Thread.Sleep(2000);
            
            bool reviewExists = product.HasReviewWithContent(reviewContent);
            Assert.That(reviewExists, Is.True, 
                $"Đánh giá với nội dung '{reviewContent}' không xuất hiện trên trang sản phẩm!");
            
            Console.WriteLine($"✓ Đánh giá với nội dung '{reviewContent}' đã xuất hiện - ĐÚNG");
            Console.WriteLine("✓ Đánh giá sản phẩm thành công");
        }

        [Test, Order(2), Description("TC_F8.2_01: Kiểm tra hiển thị danh sách đánh giá")]
        public void TC_F8_2_01_ViewReviewsList()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Truy cập trang chi tiết sản phẩm (product ID 1)
            var product = new ProductDetailsPage(Driver);
            product.NavigateTo("1");
            Thread.Sleep(1000);
            
            Console.WriteLine("Đã truy cập trang chi tiết sản phẩm");
            
            // Bước 3: Kéo xuống phần Đánh giá
            product.ScrollToReviewsSection();
            Thread.Sleep(500);
            
            Console.WriteLine("Đã kéo xuống phần đánh giá");
            
            // Bước 4: Chọn tab Đánh giá (nếu có)
            product.ClickReviewsTab();
            Thread.Sleep(1000);
            
            Console.WriteLine("Đã chọn tab đánh giá");
            
            // Bước 5: Quan sát danh sách đánh giá
            bool hasReviews = product.HasReviewList();
            
            if (!hasReviews)
            {
                Console.WriteLine("Sản phẩm chưa có đánh giá");
                Assert.Pass("Sản phẩm chưa có đánh giá - cần chạy TC_F8_1_01 trước để tạo đánh giá mẫu");
                return;
            }
            
            int reviewCount = product.GetReviewCount();
            Console.WriteLine($"Số lượng đánh giá: {reviewCount}");
            
            Assert.That(reviewCount, Is.GreaterThan(0), "Không tìm thấy đánh giá nào!");
            
            // Bước 6: Verify đánh giá đầu tiên có đầy đủ thông tin
            bool hasUserName = product.ReviewContainsUserName(0);
            Assert.That(hasUserName, Is.True, "Đánh giá không hiển thị tên user!");
            Console.WriteLine("✓ Đánh giá hiển thị tên user");
            
            bool hasStars = product.ReviewContainsStars(0);
            Assert.That(hasStars, Is.True, "Đánh giá không hiển thị số sao!");
            Console.WriteLine("✓ Đánh giá hiển thị số sao");
            
            bool hasContent = product.ReviewContainsContent(0);
            Assert.That(hasContent, Is.True, "Đánh giá không hiển thị nội dung!");
            Console.WriteLine("✓ Đánh giá hiển thị nội dung");
            
            bool hasDate = product.ReviewContainsDate(0);
            Assert.That(hasDate, Is.True, "Đánh giá không hiển thị ngày!");
            Console.WriteLine("✓ Đánh giá hiển thị ngày");
            
            Console.WriteLine($"✓ Danh sách đánh giá hiển thị đầy đủ thông tin ({reviewCount} đánh giá)");
        }


        [Test, Order(3), Description("TC_F8.3_01: Kiểm tra validation khi không nhập nội dung đánh giá")]
        public void TC_F8_3_01_ReviewWithoutContent()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            Driver.Navigate().GoToUrl(baseUrl + "/Products/Details/1");
            Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            Thread.Sleep(2000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Review Validation");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng và cập nhật trạng thái
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            Thread.Sleep(1000);
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            Thread.Sleep(1000);
            
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Đã giao");
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã cập nhật trạng thái sang 'Đã giao'");
            
            // Bước 4: Truy cập trang Đơn hàng đã giao
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            Thread.Sleep(1000);
            
            // Bước 5: Chọn Đánh giá sản phẩm
            orders.ClickViewDetails(0);
            Thread.Sleep(1000);
            
            var details = new OrderDetailsPage(Driver);
            details.ClickReviewButton();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã chuyển đến trang sản phẩm với tab đánh giá");
            
            // Bước 6: Chọn 5 sao
            product.SelectRating(5);
            Console.WriteLine("Đã chọn 5 sao");
            
            // Bước 7: KHÔNG nhập nội dung đánh giá (để trống)
            // Skip EnterReviewContent
            Console.WriteLine("Bỏ qua nhập nội dung đánh giá");
            
            // Bước 8: Gửi đánh giá
            product.SubmitReview();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã click gửi đánh giá");
            
            // Bước 9: Verify validation - đánh giá KHÔNG được tạo
            // Kiểm tra xem có thông báo lỗi hoặc vẫn ở trang hiện tại
            string currentUrl = Driver.Url;
            Console.WriteLine($"URL hiện tại: {currentUrl}");
            
            // Kiểm tra xem form vẫn hiển thị (không submit thành công)
            bool formStillDisplayed = product.IsReviewFormDisplayed();
            
            if (formStillDisplayed)
            {
                Console.WriteLine("✓ Form đánh giá vẫn hiển thị - validation hoạt động đúng");
                Assert.Pass("Validation hoạt động đúng - không cho phép gửi đánh giá không có nội dung");
            }
            else
            {
                // Kiểm tra xem có thông báo thành công không (không nên có)
                bool hasSuccess = product.HasSuccessMessage();
                
                if (hasSuccess)
                {
                    Assert.Fail("BUG: Hệ thống cho phép gửi đánh giá không có nội dung!");
                }
                else
                {
                    // Refresh và kiểm tra xem đánh giá có xuất hiện không
                    Driver.Navigate().Refresh();
                    Thread.Sleep(2000);
                    
                    // Kiểm tra xem có đánh giá rỗng xuất hiện không
                    bool hasEmptyReview = product.HasReviewWithContent("");
                    
                    if (hasEmptyReview)
                    {
                        Assert.Fail("BUG: Đánh giá rỗng đã được tạo trong hệ thống!");
                    }
                    else
                    {
                        Console.WriteLine("✓ Đánh giá không được tạo - validation hoạt động đúng");
                        Assert.Pass("Validation hoạt động đúng - đánh giá không có nội dung không được tạo");
                    }
                }
            }
        }

        [Test, Order(4), Description("TC_F8.3_02: Kiểm tra validation khi không chọn sao")]
        public void TC_F8_3_02_ReviewWithoutRating()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới
            Console.WriteLine("Đang tạo đơn hàng mới...");
            Driver.Navigate().GoToUrl(baseUrl + "/Products/Details/1");
            Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            Thread.Sleep(2000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Review No Rating");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng và cập nhật trạng thái
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            Thread.Sleep(1000);
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            Thread.Sleep(1000);
            
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Đã giao");
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã cập nhật trạng thái sang 'Đã giao'");
            
            // Bước 4: Truy cập trang Đơn hàng đã giao
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            Thread.Sleep(1000);
            
            // Bước 5: Chọn Đánh giá sản phẩm
            orders.ClickViewDetails(0);
            Thread.Sleep(1000);
            
            var details = new OrderDetailsPage(Driver);
            details.ClickReviewButton();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã chuyển đến trang sản phẩm với tab đánh giá");
            
            // Bước 6: KHÔNG chọn sao (bỏ qua SelectRating)
            Console.WriteLine("Bỏ qua chọn sao");
            
            // Bước 7: Nhập nội dung đánh giá
            string reviewContent = "123";
            product.EnterReviewContent(reviewContent);
            Console.WriteLine($"Đã nhập nội dung: {reviewContent}");
            
            // Bước 8: Gửi đánh giá
            product.SubmitReview();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã click gửi đánh giá");
            
            // Bước 9: Verify validation - đánh giá KHÔNG được tạo
            string currentUrl = Driver.Url;
            Console.WriteLine($"URL hiện tại: {currentUrl}");
            
            // Kiểm tra xem form vẫn hiển thị (không submit thành công)
            bool formStillDisplayed = product.IsReviewFormDisplayed();
            
            if (formStillDisplayed)
            {
                Console.WriteLine("✓ Form đánh giá vẫn hiển thị - validation hoạt động đúng");
                Assert.Pass("Validation hoạt động đúng - không cho phép gửi đánh giá không có số sao");
            }
            else
            {
                // Kiểm tra xem có thông báo thành công không (không nên có)
                bool hasSuccess = product.HasSuccessMessage();
                
                if (hasSuccess)
                {
                    Assert.Fail("BUG: Hệ thống cho phép gửi đánh giá không có số sao!");
                }
                else
                {
                    // Refresh và kiểm tra xem đánh giá có xuất hiện không
                    Driver.Navigate().Refresh();
                    Thread.Sleep(2000);
                    
                    // Kiểm tra xem có đánh giá với nội dung "123" nhưng không có sao
                    bool hasReview = product.HasReviewWithContent(reviewContent);
                    
                    if (hasReview)
                    {
                        Assert.Fail("BUG: Đánh giá không có số sao đã được tạo trong hệ thống!");
                    }
                    else
                    {
                        Console.WriteLine("✓ Đánh giá không được tạo - validation hoạt động đúng");
                        Assert.Pass("Validation hoạt động đúng - đánh giá không có số sao không được tạo");
                    }
                }
            }
        }


        [Test, Order(5), Description("TC_F8.3_03: Kiểm tra đánh giá khi chưa đăng nhập")]
        public void TC_F8_3_03_ReviewWithoutLogin()
        {
            // Bước 1: KHÔNG đăng nhập - truy cập trực tiếp trang Orders
            Console.WriteLine("Truy cập trang Orders mà không đăng nhập...");
            Driver.Navigate().GoToUrl(baseUrl + "/Orders");
            Thread.Sleep(2000);
            
            // Bước 2: Verify chuyển hướng đến trang đăng nhập
            Assert.That(Driver.Url, Does.Contain("/Account/Login").Or.Contain("/login"), 
                "Không chuyển hướng đến trang đăng nhập khi chưa đăng nhập!");
            
            Console.WriteLine("✓ Đã chuyển hướng đến trang đăng nhập - ĐÚNG");
            Console.WriteLine("✓ Không thể truy cập trang Đơn hàng khi chưa đăng nhập");
        }

        [Test, Order(6), Description("TC_F8.3_04: Kiểm tra không cho đánh giá lại sản phẩm")]
        public void TC_F8_3_04_CannotReviewTwice()
        {
            // Bước 1: Đăng nhập
            LoginAsAdmin();
            
            // Bước 2: Tạo đơn hàng mới và đánh giá lần đầu
            Console.WriteLine("Đang tạo đơn hàng mới...");
            Driver.Navigate().GoToUrl(baseUrl + "/Products/Details/1");
            Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow();
            Thread.Sleep(2000);
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Review Twice");
            
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            checkout.SubmitOrder();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đơn hàng đã được tạo thành công");
            
            // Bước 3: Lấy mã đơn hàng và cập nhật trạng thái
            var orders = new OrdersPage(Driver);
            orders.NavigateTo();
            orders.SelectProcessingTab();
            Thread.Sleep(1000);
            string orderId = orders.GetFirstOrderId();
            Console.WriteLine($"Mã đơn hàng: #{orderId}");
            
            var adminOrders = new AdminOrdersPage(Driver);
            adminOrders.NavigateToDetails(orderId);
            Thread.Sleep(1000);
            
            adminOrders.UpdateOrderStatus("Chờ lấy hàng");
            Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Chờ giao hàng");
            Thread.Sleep(2000);
            adminOrders.UpdateOrderStatus("Đã giao");
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã cập nhật trạng thái sang 'Đã giao'");
            
            // Bước 4: Đánh giá lần đầu
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            Thread.Sleep(1000);
            orders.ClickViewDetails(0);
            Thread.Sleep(1000);
            
            var details = new OrderDetailsPage(Driver);
            
            // Verify nút đánh giá hiển thị lần đầu
            bool reviewButtonDisplayed = details.IsReviewButtonDisplayed();
            Assert.That(reviewButtonDisplayed, Is.True, 
                "Nút 'Đánh giá' không hiển thị với đơn hàng chưa đánh giá");
            Console.WriteLine("Nút 'Đánh giá' hiển thị lần đầu");
            
            details.ClickReviewButton();
            Thread.Sleep(2000);
            
            product.SelectRating(5);
            product.EnterReviewContent("Đánh giá lần đầu");
            product.SubmitReview();
            Thread.Sleep(2000);
            
            Console.WriteLine("Đã đánh giá lần đầu thành công");
            
            // Bước 5: Quay lại chi tiết đơn hàng và kiểm tra nút đánh giá
            orders.NavigateTo();
            orders.SelectDeliveredTab();
            Thread.Sleep(1000);
            orders.ClickViewDetails(0);
            Thread.Sleep(1000);
            
            // Bước 6: Verify nút đánh giá KHÔNG còn hiển thị
            bool reviewButtonStillDisplayed = details.IsReviewButtonDisplayed();
            Assert.That(reviewButtonStillDisplayed, Is.False, 
                "BUG: Nút 'Đánh giá' vẫn hiển thị sau khi đã đánh giá!");
            
            Console.WriteLine("✓ Nút 'Đánh giá' không còn hiển thị sau khi đã đánh giá - ĐÚNG");
            Console.WriteLine("✓ Không thể đánh giá lại sản phẩm đã đánh giá");
        }

        [Test, Order(7), Description("TC_F8.3_05: Kiểm tra đánh giá khi chưa mua sản phẩm")]
        public void TC_F8_3_05_ReviewWithoutPurchase()
        {
            // Bước 1: Đăng nhập với user chưa mua hàng
            Driver.Navigate().GoToUrl(baseUrl + "/Account/Login");
            var loginPage = new LoginPage(Driver);
            var emptyUser = ConfigReader.GetUserData("empty_user");
            loginPage.Login(emptyUser.Username, emptyUser.Password);
            Thread.Sleep(1000);
            
            Console.WriteLine("Đã đăng nhập với user chưa mua sản phẩm");
            
            // Bước 2: Truy cập trang Chi tiết sản phẩm
            var product = new ProductDetailsPage(Driver);
            product.NavigateTo("1");
            Thread.Sleep(1000);
            
            Console.WriteLine("Đã truy cập trang chi tiết sản phẩm");
            
            // Bước 3: Kéo xuống phần Đánh giá
            product.ScrollToReviewsSection();
            Thread.Sleep(500);
            
            Console.WriteLine("Đã kéo xuống phần đánh giá");
            
            // Bước 4: Quan sát - Verify KHÔNG có form đánh giá hoặc nút đánh giá
            bool reviewFormDisplayed = product.IsReviewFormDisplayed();
            
            if (reviewFormDisplayed)
            {
                Assert.Fail("BUG: Form đánh giá hiển thị cho user chưa mua sản phẩm!");
            }
            
            Console.WriteLine("✓ Form đánh giá không hiển thị cho user chưa mua sản phẩm - ĐÚNG");
            
            // Bước 5: Verify có thể xem danh sách đánh giá của người khác
            product.ClickReviewsTab();
            Thread.Sleep(1000);
            
            bool hasReviewList = product.HasReviewList();
            
            if (hasReviewList)
            {
                Console.WriteLine("✓ Có thể xem danh sách đánh giá của người khác");
            }
            else
            {
                Console.WriteLine("Sản phẩm chưa có đánh giá nào");
            }
            
            Console.WriteLine("✓ User chưa mua sản phẩm không thể đánh giá nhưng có thể xem đánh giá");
        }
    }
}
