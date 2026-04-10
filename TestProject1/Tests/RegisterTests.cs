using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumProject.Pages;
using TestProject1.Utilities;
using System;
using System.Linq;

namespace TestProject1.Tests
{
    [TestFixture]
    public class RegisterTests : BaseTest
    {
        private IWebDriver Driver => driver!;
        private const string RegisterUrl = "https://shop-production-b6d0.up.railway.app/Account/Register";

        [SetUp]
        public override void Setup()
        {
            driver = DriverFactory.InitDriver();
        }

        // Helper: tạo page đã ở trang đăng ký
        private RegisterPage GoToRegister()
        {
            Driver.Navigate().GoToUrl(RegisterUrl);
            return new RegisterPage(Driver);
        }

        // Helper: kiểm tra thất bại (vẫn ở trang đăng ký và/hoặc có lỗi)
        private static void AssertRegisterFailed(RegisterPage reg)
        {
            Assert.That(
                reg.IsStillOnRegisterPage() || reg.HasRegisterError() || reg.HasClientSideValidationError(),
                Is.True, $"Expected register to fail. Errors: {reg.GetRegisterErrors()}");
        }

        // ==========================================
        // F1.1: ĐĂNG KÝ THÀNH CÔNG
        // ==========================================

        [Test, Description("TC_F1.1_01: Đăng ký thành công với thông tin hợp lệ")]
        public void TC_F1_1_01()
        {
            // Dùng email và SĐT ngẫu nhiên để tránh trùng khi chạy lại
            string email = $"test{DateTime.Now.Ticks}@gmail.com";
            string phone = GeneratePhone();
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", email, "Test@123", "Test@123", phone, "Nam");
            Assert.Multiple(() =>
            {
                // Web redirect sang /Account/Login sau khi đăng ký OK
                // → chỉ cần kiểm tra đã thoát khỏi trang /Register
                Assert.That(reg.IsRegisterSuccess(), Is.True, "Vẫn còn ở trang đăng ký sau khi submit");
                Assert.That(reg.HasRegisterError() && reg.IsStillOnRegisterPage(), Is.False,
                    $"Đăng ký thất bại - lỗi: {reg.GetRegisterErrors()}");
            });
        }

        // ==========================================
        // F1.2: ĐĂNG KÝ THẤT BẠI
        // ==========================================

        [Test, Description("TC_F1.2_01: Đăng ký bỏ trống tất cả trường")]
        public void TC_F1_2_01()
        {
            var reg = GoToRegister();
            reg.Register("", "", "", "", "", "");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_02: Đăng ký bỏ trống trường Họ và Tên")]
        public void TC_F1_2_02()
        {
            var reg = GoToRegister();
            // Họ và Tên bỏ trống, các trường khác hợp lệ
            reg.FillForm("", "shopclothing1525@gmail.com", "Test@123", "Test@123", "0123456789", "Nam");
            reg.ClickCreateAccountOnly();
            System.Threading.Thread.Sleep(2000);
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_03: Đăng ký với Email sai định dạng (abc123)")]
        public void TC_F1_2_03()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "abc123", "Test@123", "Test@123", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_04: Đăng ký bỏ trống trường Email")]
        public void TC_F1_2_04()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "", "Test@123", "Test@123", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_05: Đăng ký với Email chứa khoảng trắng")]
        public void TC_F1_2_05()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing 1525@gmail.com", "Test@123", "Test@123", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_06: Đăng ký với Email đã tồn tại trong hệ thống")]
        public void TC_F1_2_06()
        {
            // Dùng email của tài khoản "user" đã có trong hệ thống
            var existingUser = ConfigReader.GetUserData("user");
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", existingUser.Username, "Test@123", "Test@123", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_07: Đăng ký với SĐT sai định dạng (abc123)")]
        public void TC_F1_2_07()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", "Test@123", "Test@123", "abc123", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_08: Đăng ký bỏ trống trường SĐT")]
        public void TC_F1_2_08()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", "Test@123", "Test@123", "", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_09: Đăng ký với SĐT chứa khoảng trắng")]
        public void TC_F1_2_09()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", "Test@123", "Test@123", "0123 56789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_10: Đăng ký với Mật khẩu không khớp với Xác nhận mật khẩu")]
        public void TC_F1_2_10()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", "Testdung@123", "Testsai@123", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_11: Đăng ký với Mật khẩu quá yếu (dưới 6 kí tự)")]
        public void TC_F1_2_11()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", "123", "123", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_12: Đăng ký bỏ trống trường Mật khẩu")]
        public void TC_F1_2_12()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", "", "", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.2_13: Đăng ký với SĐT vượt quá độ dài cho phép (11 chữ số)")]
        public void TC_F1_2_13()
        {
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", "Test@123", "Test@123", "01234567890", "Nam");
            AssertRegisterFailed(reg);
        }

        // ==========================================
        // F1.4: KIỂM TRA GIỚI HẠN KÝ TỰ
        // ==========================================

        [Test, Description("TC_F1.4_01: Kiểm tra giới hạn ký tự trường Họ và Tên (100+ ký tự)")]
        public void TC_F1_4_01()
        {
            string longName = new string('a', 105);
            var reg = GoToRegister();
            reg.Register(longName, "shopclothing1525@gmail.com", "Test@123", "Test@123", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.4_02: Kiểm tra giới hạn ký tự trường Email (100+ ký tự)")]
        public void TC_F1_4_02()
        {
            string longEmail = new string('a', 100) + "@gmail.com";
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", longEmail, "Test@123", "Test@123", "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.4_03: Kiểm tra giới hạn ký tự trường Mật khẩu (100+ ký tự)")]
        public void TC_F1_4_03()
        {
            string longPass = "Aaa" + new string('a', 100) + "@123";
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", longPass, longPass, "0123456789", "Nam");
            AssertRegisterFailed(reg);
        }

        [Test, Description("TC_F1.4_04: Kiểm tra giới hạn ký tự trường SĐT (100+ ký tự)")]
        public void TC_F1_4_04()
        {
            string longPhone = "012" + new string('1', 100);
            var reg = GoToRegister();
            reg.Register("Nguyen Van A", "shopclothing1525@gmail.com", "Test@123", "Test@123", longPhone, "Nam");
            AssertRegisterFailed(reg);
        }

        private static string GeneratePhone()
        {
            Random rand = new();
            string phone = "0";
            for (int i = 0; i < 9; i++)
                phone += rand.Next(0, 10);
            return phone;
        }
    }
}
