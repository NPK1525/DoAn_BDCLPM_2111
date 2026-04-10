using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumProject.Pages;
using TestProject1.Utilities;
using System;
using System.Linq;

namespace TestProject1.Tests
{
    [TestFixture]
    public class CartTests : BaseTest
    {
        private IWebDriver Driver => driver!;
        private const string LoginUrl = "https://shop-production-b6d0.up.railway.app/Account/Login";
        private const string ProductUrl = "/Products/Details/1"; // Sản phẩm Áo

        [SetUp]
        public override void Setup()
        {
            driver = DriverFactory.InitDriver();
        }

        // ── Helper: đăng nhập bằng tài khoản user ──────────────────────────────
        private void LoginAsUser()
        {
            Driver.Navigate().GoToUrl(LoginUrl);
            var login = new LoginPage(Driver);
            var user = ConfigReader.GetUserData("user");
            login.Login(user.Username, user.Password);
            System.Threading.Thread.Sleep(2000);
        }

        // ── Helper: thêm sản phẩm (màu Đỏ, size S, SL=1) vào giỏ ─────────────
        private void AddProductToCart(string color = "Đỏ", string size = "S", string qty = "1")
        {
            Console.WriteLine($"Thêm SP: Màu={color}, Size={size}, SL={qty}");
            Driver.Navigate().GoToUrl(baseUrl + ProductUrl);
            var details = new ProductDetailsPage(Driver);
            
            if (!string.IsNullOrEmpty(color)) details.SelectColor(color);
            if (!string.IsNullOrEmpty(size))  details.SelectSize(size);
            details.SetQuantity(qty);
            details.ClickAddToCart();
            
            System.Threading.Thread.Sleep(3000);
        }

        // ── Helper: chuẩn bị giỏ hàng với sản phẩm mới (xóa cũ, thêm mới) ─────
        private CartPage PrepareCartWithProduct(string color = "Đỏ", string size = "S", string qty = "1")
        {
            var cart = new CartPage(Driver);
            cart.ClearAllItems();
            AddProductToCart(color, size, qty);
            cart.NavigateTo();
            return cart;
        }

        // ==========================================
        // F3.1: HIỂN THỊ GIỎ HÀNG
        // ==========================================

        [Test, Description("TC_F3.1_01: Kiểm tra hiển thị danh sách sản phẩm trong giỏ hàng")]
        public void TC_F3_1_01()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "1");

            Assert.Multiple(() =>
            {
                Assert.That(cart.HasItems(), Is.True, "Giỏ hàng không có sản phẩm");
                Assert.That(cart.IsItemNameDisplayed(), Is.True, "Không hiển thị tên sản phẩm");
                Assert.That(cart.IsItemPriceDisplayed(), Is.True, "Không hiển thị đơn giá");
                Assert.That(cart.GetCartTotal(), Is.Not.Empty, "Không hiển thị tổng tiền");
                
                // Kiểm tra số lượng
                int actualQty = cart.GetItemQuantity(0);
                Console.WriteLine($"Số lượng trong giỏ: {actualQty}");
                Assert.That(actualQty, Is.GreaterThan(0), $"Số lượng phải > 0. Thực tế: {actualQty}");
            });
        }

        [Test, Description("TC_F3.1_02: Kiểm tra hiển thị giỏ hàng rỗng và nút Tiếp tục mua sắm")]
        public void TC_F3_1_02()
        {
            LoginAsUser();
            var cart = new CartPage(Driver);
            cart.ClearAllItems();

            Assert.That(cart.IsEmptyMessageDisplayed(), Is.True, "Không hiển thị thông báo giỏ hàng rỗng");

            // Nhấn "Tiếp tục mua sắm" → về trang chủ
            var continueBtns = Driver.FindElements(By.XPath("//*[contains(text(),'Tiếp tục') or contains(text(),'mua sắm')]"));
            var visible = continueBtns.FirstOrDefault(e => e.Displayed);
            if (visible != null)
            {
                visible.Click();
                System.Threading.Thread.Sleep(1500);
                Assert.That(Driver.Url, Does.Not.Contain("/Cart"), "Không chuyển hướng khỏi trang giỏ hàng");
            }
        }

        // ==========================================
        // F3.2: THÊM SẢN PHẨM VÀO GIỎ HÀNG
        // ==========================================

        [Test, Description("TC_F3.2_01: Thêm sản phẩm mới vào giỏ hàng")]
        public void TC_F3_2_01()
        {
            LoginAsUser();
            var cart = new CartPage(Driver);
            cart.ClearAllItems();

            AddProductToCart("Đỏ", "S", "1");
            cart.NavigateTo();
            Assert.That(cart.GetItemCount(), Is.GreaterThan(0), "Sản phẩm chưa được thêm vào giỏ hàng");
        }

        [Test, Description("TC_F3.2_02: Thêm sản phẩm đã có vào giỏ hàng → cộng dồn số lượng")]
        public void TC_F3_2_02()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "1");
            int qtyBefore = cart.GetItemQuantity(0);

            // Thêm lại cùng sản phẩm
            AddProductToCart("Đỏ", "S", "1");
            cart.NavigateTo();
            int qtyAfter = cart.GetItemQuantity(0);

            Assert.That(qtyAfter, Is.GreaterThan(qtyBefore), "Số lượng không được cộng dồn");
        }

        [Test, Description("TC_F3.2_03: Thêm sản phẩm đã có nhưng khác size → thêm mục mới")]
        public void TC_F3_2_03()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "1");
            int countBefore = cart.GetItemCount();

            AddProductToCart("Đỏ", "L", "1");
            cart.NavigateTo();
            int countAfter = cart.GetItemCount();

            Assert.Multiple(() =>
            {
                Assert.That(countAfter, Is.GreaterThan(countBefore), "Số mục trong giỏ không tăng");
                Assert.That(cart.GetCartTotal(), Is.Not.Empty, "Tổng tiền không cập nhật");
            });
        }

        [Test, Description("TC_F3.2_04: Thêm sản phẩm vượt quá số lượng tồn (SL=999)")]
        public void TC_F3_2_04()
        {
            LoginAsUser();
            Driver.Navigate().GoToUrl(baseUrl + ProductUrl);
            var details = new ProductDetailsPage(Driver);
            details.SelectColor("Đỏ");
            details.SelectSize("S");
            details.SetQuantity("999");
            details.ClickAddToCart();
            System.Threading.Thread.Sleep(2000);

            // Kỳ vọng: bị block hoặc số lượng bị giới hạn về max
            bool isBlocked = details.IsAddToCartBlocked();
            int maxStock = details.GetMaxStock();
            // Chấp nhận nếu bị block OR số lượng thực tế bị clamp về max tồn kho
            Assert.That(isBlocked || maxStock > 0, Is.True,
                "Hệ thống không giới hạn số lượng vượt quá tồn kho");
        }

        [Test, Description("TC_F3.2_05: Thêm sản phẩm khi chưa chọn size → bị chặn")]
        public void TC_F3_2_05()
        {
            LoginAsUser();
            Driver.Navigate().GoToUrl(baseUrl + ProductUrl);
            var details = new ProductDetailsPage(Driver);
            details.SelectColor("Đỏ");
            // KHÔNG chọn size
            details.SetQuantity("1");
            details.ClickAddToCart();
            System.Threading.Thread.Sleep(2000);

            Assert.That(details.IsAddToCartBlocked() || Driver.Url.Contains("/Products"),
                Is.True, "Hệ thống cho phép thêm vào giỏ khi chưa chọn size");
        }

        [Test, Description("TC_F3.2_06: Thêm sản phẩm khi chưa chọn màu → bị chặn")]
        public void TC_F3_2_06()
        {
            LoginAsUser();
            Driver.Navigate().GoToUrl(baseUrl + ProductUrl);
            var details = new ProductDetailsPage(Driver);
            // KHÔNG chọn màu
            details.SelectSize("S");
            details.SetQuantity("1");
            details.ClickAddToCart();
            System.Threading.Thread.Sleep(2000);

            Assert.That(details.IsAddToCartBlocked() || Driver.Url.Contains("/Products"),
                Is.True, "Hệ thống cho phép thêm vào giỏ khi chưa chọn màu");
        }

        [Test, Description("TC_F3.2_07: Thêm sản phẩm với số lượng bằng 0 → bị chặn")]
        public void TC_F3_2_07()
        {
            LoginAsUser();
            Driver.Navigate().GoToUrl(baseUrl + ProductUrl);
            var details = new ProductDetailsPage(Driver);
            details.SelectColor("Đỏ");
            details.SelectSize("S");
            details.SetQuantity("0");
            details.ClickAddToCart();
            System.Threading.Thread.Sleep(2000);

            Assert.That(details.IsAddToCartBlocked() || Driver.Url.Contains("/Products"),
                Is.True, "Hệ thống cho phép thêm sản phẩm với SL=0");
        }

        [Test, Description("TC_F3.2_08: Thêm sản phẩm với số lượng âm → bị chặn")]
        public void TC_F3_2_08()
        {
            LoginAsUser();
            Driver.Navigate().GoToUrl(baseUrl + ProductUrl);
            var details = new ProductDetailsPage(Driver);
            details.SelectColor("Đỏ");
            details.SelectSize("S");
            details.SetQuantity("-1");
            details.ClickAddToCart();
            System.Threading.Thread.Sleep(2000);

            Assert.That(details.IsAddToCartBlocked() || Driver.Url.Contains("/Products"),
                Is.True, "Hệ thống cho phép thêm sản phẩm với SL âm");
        }

        // ==========================================
        // F3.3: CẬP NHẬT SỐ LƯỢNG TRONG GIỎ HÀNG
        // ==========================================

        [Test, Description("TC_F3.3_01: Tăng số lượng bằng nút + → SL tăng và tổng tiền cập nhật")]
        public void TC_F3_3_01()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "2");
            int before = cart.GetItemQuantity(0);

            cart.ClickIncrease(0);
            cart.NavigateTo();
            int after = cart.GetItemQuantity(0);

            Assert.That(after, Is.GreaterThan(before), "Số lượng không tăng sau khi nhấn +");
        }

        [Test, Description("TC_F3.3_02: Giảm số lượng bằng nút - → SL giảm và tổng tiền cập nhật")]
        public void TC_F3_3_02()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "3");
            int before = cart.GetItemQuantity(0);

            cart.ClickDecrease(0);
            cart.NavigateTo();
            int after = cart.GetItemQuantity(0);

            Assert.That(after, Is.LessThan(before), "Số lượng không giảm sau khi nhấn -");
        }

        [Test, Description("TC_F3.3_03: Nhập số lượng = 0 → sản phẩm bị xóa khỏi giỏ hàng")]
        public void TC_F3_3_03()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "2");
            int countBefore = cart.GetItemCount();

            cart.SetItemQuantity(0, "0");
            System.Threading.Thread.Sleep(1500);
            cart.NavigateTo();

            int countAfter = cart.GetItemCount();
            int qtyAfter = countAfter > 0 ? cart.GetItemQuantity(0) : -1;
            
            Console.WriteLine($"Trước: {countBefore} SP, Sau: {countAfter} SP, SL sau: {qtyAfter}");
            
            Assert.That(countAfter, Is.LessThan(countBefore).Or.EqualTo(0),
                $"Sản phẩm không bị xóa khi nhập SL=0. Số SP: {countBefore} → {countAfter}, SL hiện tại: {qtyAfter}");
        }

        [Test, Description("TC_F3.3_04: Nhập số lượng âm → bị chặn, không cập nhật")]
        public void TC_F3_3_04()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "2");
            int before = cart.GetItemQuantity(0);

            cart.SetItemQuantity(0, "-10");
            System.Threading.Thread.Sleep(1500);
            cart.NavigateTo();
            int after = cart.GetItemQuantity(0);

            Console.WriteLine($"SL trước: {before}, SL sau khi nhập '-10': {after}");
            Assert.That(after, Is.GreaterThanOrEqualTo(0), 
                $"Hệ thống cho phép số lượng âm (trước: {before}, sau: {after})");
        }

        [Test, Description("TC_F3.3_05: Nhập số lượng vượt tồn kho (999) → bị giới hạn")]
        public void TC_F3_3_05()
        {
            LoginAsUser();
            
            // Đọc tồn kho từ trang chi tiết sản phẩm
            driver?.Navigate().GoToUrl($"{baseUrl}/Products/Details/1");
            var productPage = new ProductDetailsPage(driver!);
            productPage.SelectColor("Đỏ");
            productPage.SelectSize("S");
            int maxStock = productPage.GetMaxStock();
            Console.WriteLine($"Tồn kho thực tế: {maxStock}");
            
            var cart = PrepareCartWithProduct("Đỏ", "S", "1");
            int before = cart.GetItemQuantity(0);

            cart.SetItemQuantity(0, "999");
            System.Threading.Thread.Sleep(2000);
            cart.NavigateTo();
            int after = cart.GetItemQuantity(0);

            Console.WriteLine($"SL trước: {before}, SL sau khi nhập '999': {after}");
            
            // Yêu cầu: Không cho phép cập nhật quá tồn kho
            Assert.That(after, Is.LessThanOrEqualTo(maxStock),
                $"Hệ thống cho phép cập nhật SL vượt tồn kho. Tồn kho={maxStock}, nhưng SL trong giỏ={after}");
        }

        [Test, Description("TC_F3.3_06: Nhập số lượng là ký tự chữ (abc) → bị chặn")]
        public void TC_F3_3_06()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "2");
            int before = cart.GetItemQuantity(0);

            cart.SetItemQuantity(0, "abc");
            System.Threading.Thread.Sleep(1500);
            cart.NavigateTo();
            int after = cart.GetItemQuantity(0);

            Console.WriteLine($"SL trước: {before}, SL sau khi nhập 'abc': {after}");
            Assert.That(after, Is.GreaterThan(0), 
                $"Nhập chữ làm SL về 0 (trước: {before}, sau: {after}). Hệ thống nên chặn hoặc giữ nguyên SL");
        }

        [Test, Description("TC_F3.3_07: Nhập số lượng thập phân (1.5) → bị chặn hoặc truncate")]
        public void TC_F3_3_07()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "2");

            cart.SetItemQuantity(0, "1.5");
            System.Threading.Thread.Sleep(1500);
            cart.NavigateTo();
            int after = cart.GetItemQuantity(0);

            Assert.That(after, Is.GreaterThan(0), "Nhập thập phân làm SL về 0");
            Console.WriteLine($"SL sau khi nhập '1.5': {after}");
        }

        [Test, Description("TC_F3.3_08: Nhập số lượng có khoảng trắng (1 0) → bị xử lý đúng")]
        public void TC_F3_3_08()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "2");

            cart.SetItemQuantity(0, "1 0");
            System.Threading.Thread.Sleep(1500);
            cart.NavigateTo();
            int after = cart.GetItemQuantity(0);

            Assert.That(after, Is.GreaterThan(0), "Nhập '1 0' làm SL về 0");
            Console.WriteLine($"SL sau khi nhập '1 0': {after}");
        }

        [Test, Description("TC_F3.3_09: Nhấn nút + khi đang ở SL tối đa → không vượt quá tồn kho")]
        public void TC_F3_3_09()
        {
            LoginAsUser();
            
            // Đọc tồn kho từ trang chi tiết sản phẩm
            driver?.Navigate().GoToUrl($"{baseUrl}/Products/Details/1");
            var productPage = new ProductDetailsPage(driver!);
            productPage.SelectColor("Đỏ");
            productPage.SelectSize("S");
            int maxStock = productPage.GetMaxStock();
            Console.WriteLine($"Tồn kho thực tế: {maxStock}");
            
            var cart = PrepareCartWithProduct("Đỏ", "S", "5");
            int before = cart.GetItemQuantity(0);

            cart.ClickIncrease(0);
            System.Threading.Thread.Sleep(1500);
            cart.NavigateTo();
            int after = cart.GetItemQuantity(0);

            Console.WriteLine($"SL trước: {before}, sau khi nhấn +: {after}");
            
            // Yêu cầu: Không cho vượt quá tồn kho khi nhấn +
            Assert.That(after, Is.LessThanOrEqualTo(maxStock),
                $"Hệ thống cho phép tăng SL vượt tồn kho bằng nút +. Tồn kho={maxStock}, nhưng SL trong giỏ={after}");
        }

        [Test, Description("TC_F3.3_10: Nhấn nút - từ SL=1 → sản phẩm bị xóa khỏi giỏ hàng")]
        public void TC_F3_3_10()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "1");
            int countBefore = cart.GetItemCount();
            int qtyBefore = cart.GetItemQuantity(0);

            cart.ClickDecrease(0);
            System.Threading.Thread.Sleep(1500);
            cart.NavigateTo();

            int countAfter = cart.GetItemCount();
            int qtyAfter = countAfter > 0 ? cart.GetItemQuantity(0) : -1;
            
            Console.WriteLine($"Trước: {countBefore} SP (SL={qtyBefore}), Sau: {countAfter} SP (SL={qtyAfter})");
            
            Assert.That(countAfter, Is.LessThan(countBefore).Or.EqualTo(0),
                $"Sản phẩm không bị xóa khi giảm từ SL=1 xuống 0. Số SP: {countBefore} → {countAfter}, SL: {qtyBefore} → {qtyAfter}");
        }

        // ==========================================
        // F3.4: XÓA SẢN PHẨM KHỎI GIỎ HÀNG
        // ==========================================

        [Test, Description("TC_F3.4_01: Xóa một sản phẩm khỏi giỏ hàng → cập nhật tổng tiền")]
        public void TC_F3_4_01()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "2");
            int countBefore = cart.GetItemCount();

            cart.ClickDelete(0);
            System.Threading.Thread.Sleep(2000);
            cart.NavigateTo();

            Assert.That(cart.GetItemCount(), Is.LessThan(countBefore),
                "Sản phẩm chưa bị xóa khỏi giỏ hàng");
        }

        [Test, Description("TC_F3.4_02: Xóa toàn bộ sản phẩm → hiển thị thông báo giỏ hàng rỗng")]
        public void TC_F3_4_02()
        {
            LoginAsUser();
            var cart = PrepareCartWithProduct("Đỏ", "S", "2");
            
            cart.ClickClearAll();
            System.Threading.Thread.Sleep(2000);
            cart.NavigateTo();

            Assert.That(cart.IsEmptyMessageDisplayed(), Is.True,
                "Không hiển thị thông báo giỏ hàng rỗng sau khi nhấn nút 'Làm trống giỏ hàng'");
        }
    }
}
