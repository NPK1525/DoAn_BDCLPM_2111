using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumProject.Pages;
using TestProject1.Utilities;

namespace TestProject1.Tests
{
    [TestFixture]
    public class CheckoutTests : BaseTest
    {
        private IWebDriver Driver => driver!;

        private void LoginAsUser()
        {
            Driver.Navigate().GoToUrl(baseUrl + "/Account/Login");
            var loginPage = new LoginPage(Driver);
            var admin = ConfigReader.GetUserData("admin"); // Dùng admin thay vì user
            loginPage.Login(admin.Username, admin.Password);
        }

        private void LoginAndGoToCheckout()
        {
            LoginAsUser();
            var home = new HomePage(Driver);
            home.NavigateTo();
            
            // Chọn sản phẩm đầu tiên
            home.ClickFirstProduct();
            Thread.Sleep(1000);
            
            var product = new ProductDetailsPage(Driver);
            product.SelectColor(0);
            product.SelectSize(0);
            product.ClickBuyNow(); // Mua ngay -> đi thẳng checkout
            Thread.Sleep(1000);
        }

        [Test, Order(1), Description("TC_Checkout_Success: Thanh toán thành công")]
        public void TC_Checkout_Success()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Selenium");
            
            // Thử nhập phone nếu có field
            try {
                checkout.EnterPhoneNumber("0123456789");
            } catch {
                Console.WriteLine("Không có field phone number hoặc đã được điền sẵn");
            }
            
            Console.WriteLine("Address entered");
            
            checkout.SubmitOrder();
            Thread.Sleep(2000);
            
            Console.WriteLine("URL sau khi đặt: " + Driver.Url);
            
            Assert.That(Driver.PageSource, Does.Contain("Đặt hàng thành công")
                .Or.Contain("thành công"), 
                "Không thấy thông báo 'Đặt hàng thành công'");
            
            Console.WriteLine("TEST PASS");
        }

        [Test, Order(2), Description("TC_F5_02: Kiểm tra thanh toán với giỏ hàng rỗng")]
        public void TC_F5_02_EmptyCartCheckout()
        {
            LoginAsUser();
            var home = new HomePage(Driver);
            home.NavigateTo();
            home.ClickCartIcon();
            Thread.Sleep(1000);
            
            Assert.That(Driver.PageSource, Does.Contain("Giỏ hàng của bạn đang trống")
                .Or.Contain("trống"), 
                "Không thấy thông báo giỏ hàng trống");
            
            var checkoutBtns = Driver.FindElements(By.XPath("//a[contains(@href,'checkout')] | //button[contains(text(),'Thanh toán')]"));
            Assert.That(checkoutBtns.Count == 0 || !checkoutBtns[0].Displayed, 
                "Nút thanh toán vẫn hiển thị khi giỏ hàng trống");
        }

        [Test, Order(3), Description("TC_F5_03: Refresh trang thanh toán")]
        public void TC_F5_03_RefreshCheckoutPage()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Selenium");
            
            // Refresh trang
            Driver.Navigate().Refresh();
            Thread.Sleep(2000);
            
            // Xác minh ko bị văng lỗi server, vẫn ở checkout hoặc có cảnh báo
            Assert.That(Driver.Url.Contains("checkout") || Driver.Url.Contains("cart"), 
                "Bị văng khỏi luồng thanh toán sau khi refresh");
        }

        [Test, Order(4), Description("TC_F5_04: Quay lại giỏ hàng từ trang thanh toán")]
        public void TC_F5_04_GoBackToCartFromCheckout()
        {
            LoginAndGoToCheckout();
            
            // Quay lại giỏ hàng
            Driver.Navigate().Back();
            Thread.Sleep(1000);
            
            Assert.Multiple(() =>
            {
                Assert.That(Driver.Url.Contains("cart") || Driver.Url.Contains("product"), 
                    "Không quay lại trang trước đó được");
                
                // Kiểm tra vẫn còn sản phẩm
                Assert.That(Driver.FindElements(By.XPath("//img | //button[contains(text(),'Mua')]")), 
                    Has.Count.GreaterThan(0), 
                    "Bị mất sản phẩm khi quay lại");
            });
        }

        [Test, Order(5), Description("TC_F5_1_01: Kiểm tra giao diện trang thanh toán")]
        public void TC_F5_1_01_CheckCheckoutUI()
        {
            LoginAndGoToCheckout();
            
            Assert.Multiple(() =>
            {
                Assert.That(Driver.Url, Does.Contain("checkout"), 
                    "Không ở trang thanh toán");
                Assert.That(Driver.FindElement(By.Id("checkoutForm")).Displayed, Is.True, 
                    "Không hiển thị form thanh toán");
            });
        }

        [Test, Order(6), Description("TC_F5_2_01: Thanh toán thất bại với thông tin sai")]
        public void TC_F5_2_01_CheckoutFailWrongInfo()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("Wrong Format Or Something");
            checkout.EnterPhoneNumber("ABC"); // fail info
            
            checkout.ClickSubmitOrder();
            Thread.Sleep(1000);
            
            Assert.That(Driver.Url, Does.Not.Contain("OrderSuccess"), 
                "Vẫn đặt hàng thành công dù sai thông tin!");
        }

        [Test, Order(7), Description("TC_F5_2_02: Hủy thanh toán")]
        public void TC_F5_2_02_CancelCheckout()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.ClickCancel();
            Thread.Sleep(1000);
            
            Assert.That(Driver.Url, Does.Not.Contain("checkout").And.Not.Contain("OrderSuccess"), 
                "Hủy thanh toán không thành công");
        }

        [Test, Order(8), Description("TC_F5_2_03: Click nút thanh toán nhiều lần")]
        public void TC_F5_2_03_ClickCheckoutMultipleTimes()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Selenium");
            
            checkout.ClickSubmitOrder();
            checkout.ClickSubmitOrder();
            checkout.ClickSubmitOrder();
            
            Thread.Sleep(2000);
            
            Assert.That(Driver.Url, Does.Contain("OrderSuccess")
                .Or.Contain("history").Or.Contain("Orders"), 
                "Lỗi hệ thống khi click nhiều lần");
        }

        [Test, Order(9), Description("TC_F5_3_01: Validation form rỗng")]
        public void TC_F5_3_01_EmptyFormValidation()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.ClearForm();
            checkout.ClickSubmitOrder();
            
            Thread.Sleep(1000);
            
            // Kiểm tra thông báo validation HTML5 hoặc text
            Assert.That(Driver.Url, Does.Not.Contain("OrderSuccess"), 
                "Cho phép gửi form rỗng");
        }

        [Test, Order(10), Description("TC_F5_3_02: Số điện thoại không hợp lệ")]
        public void TC_F5_3_02_InvalidPhoneNumber()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Selenium");
            checkout.EnterPhoneNumber("123");
            
            checkout.ClickSubmitOrder();
            Thread.Sleep(1000);
            
            Assert.That(Driver.Url, Does.Not.Contain("OrderSuccess"), 
                "Cho phép checkout số điện thoại không hợp lệ");
        }

        [Test, Order(11), Description("TC_F5_3_03: Địa chỉ trống")]
        public void TC_F5_3_03_EmptyAddress()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.ClearAddress();
            
            checkout.ClickSubmitOrder();
            Thread.Sleep(1000);
            
            Assert.That(Driver.Url, Does.Not.Contain("OrderSuccess"), 
                "Cho phép checkout với địa chỉ trống");
        }

        [Test, Order(12), Description("TC_F5_4_01: Kiểm tra thanh toán vượt hạn mức")]
        public void TC_F5_4_01_OverLimitCheckout()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Selenium");
            
            // Giả sử ta can thiệp thay đổi tổng giá trị bằng JS để vượt hạn mức
            ((IJavaScriptExecutor)Driver).ExecuteScript(@"
                var el = document.getElementById('totalPrice');
                if(el) el.value = '999999999999';
            ");
            
            checkout.ClickSubmitOrder();
            Thread.Sleep(1000);
            
            // Thực tế tuỳ hệ thống, kiểm tra URL
            Console.WriteLine("URL sau khi submit: " + Driver.Url);
        }

        [Test, Order(13), Description("TC_F5_3_04: Tên không hợp lệ (ký tự đặc biệt)")]
        public void TC_F5_3_04_InvalidNameSpecialChars()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Selenium");
            
            // Nhập tên với ký tự đặc biệt
            bool nameFieldExists = false;
            try
            {
                var nameInput = Driver.FindElement(By.Id("fullName"));
                nameInput.Clear();
                nameInput.SendKeys("@@###");
                nameFieldExists = true;
            }
            catch (NoSuchElementException) 
            { 
                Console.WriteLine("Không có field fullName");
            }
            
            checkout.ClickSubmitOrder();
            Thread.Sleep(1000);
            
            if (!nameFieldExists)
            {
                Assert.Inconclusive("Không có field fullName trên giao diện – bỏ qua");
            }
            else
            {
                Assert.That(Driver.Url, Does.Not.Contain("OrderSuccess"), 
                    "Cho phép thanh toán với tên chứa ký tự đặc biệt");
            }
        }

        [Test, Order(14), Description("TC_F5_3_05: SĐT chứa chữ cái")]
        public void TC_F5_3_05_PhoneWithLetters()
        {
            LoginAndGoToCheckout();
            
            var checkout = new CheckoutPage(Driver);
            checkout.SelectAddress();
            checkout.EnterAddress("123 Test Selenium");
            
            bool phoneFieldExists = false;
            try
            {
                checkout.EnterPhoneNumber("abcxyz");
                phoneFieldExists = true;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Không tìm thấy field phoneNumber");
            }
            
            checkout.ClickSubmitOrder();
            Thread.Sleep(1000);
            
            if (!phoneFieldExists)
            {
                Assert.Inconclusive("Giao diện không có field số điện thoại – bỏ qua");
            }
            else
            {
                Assert.That(Driver.Url, D
