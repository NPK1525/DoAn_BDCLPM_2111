using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumProject.Pages;
using TestProject1.Utilities;
using System;
using System.Linq;

namespace TestProject1.Tests
{
    [TestFixture]
    public class LoginTests : BaseTest
    {
        private IWebDriver Driver => driver!;
        private const string LoginUrl = "https://shop-production-b6d0.up.railway.app/Account/Login";
        private const string ProfileUrl = "https://shop-production-b6d0.up.railway.app/Account/Profile";

        [SetUp]
        public override void Setup()
        {
            driver = DriverFactory.InitDriver();
        }

        // Helper
        private LoginPage GoToLogin()
        {
            Driver.Navigate().GoToUrl(LoginUrl);
            return new LoginPage(Driver);
        }

        private static void AssertLoginFailed(LoginPage login)
        {
            Assert.That(
                login.IsStillOnLoginPage() || login.HasLoginError() || login.HasClientSideValidationError(),
                Is.True, "Expected login to fail but it did not.");
        }

        // ==========================================
        // F1.5: ĐĂNG NHẬP THÀNH CÔNG
        // ==========================================

        [Test, Description("TC_F1.5_01: Đăng nhập với thông tin hợp lệ")]
        public void TC_F1_5_01()
        {
            var login = GoToLogin();
            var user = ConfigReader.GetUserData("user");
            login.Login(user.Username, user.Password);
            Assert.That(login.IsLoginSuccess(), Is.True, "Đăng nhập không thành công");
        }

        // ==========================================
        // F1.6: ĐĂNG NHẬP THẤT BẠI
        // ==========================================

        [Test, Description("TC_F1.6_01: Đăng nhập để trống tất cả các trường")]
        public void TC_F1_6_01()
        {
            var login = GoToLogin();
            login.Login("", "");
            AssertLoginFailed(login);
        }

        [Test, Description("TC_F1.6_02: Đăng nhập để trống trường Email")]
        public void TC_F1_6_02()
        {
            var login = GoToLogin();
            login.Login("", "Test@123");
            AssertLoginFailed(login);
        }

        [Test, Description("TC_F1.6_03: Đăng nhập để trống trường Mật khẩu")]
        public void TC_F1_6_03()
        {
            var login = GoToLogin();
            login.Login("shopclothing1525@gmail.com", "");
            AssertLoginFailed(login);
        }

        [Test, Description("TC_F1.6_04: Đăng nhập với Email không tồn tại trong hệ thống")]
        public void TC_F1_6_04()
        {
            var login = GoToLogin();
            login.Login("kotontai@gmail.com", "Test@123");
            AssertLoginFailed(login);
        }

        [Test, Description("TC_F1.6_05: Đăng nhập với Email có khoảng trắng")]
        public void TC_F1_6_05()
        {
            var login = GoToLogin();
            login.Login("shopclothing1525 @gmail.com", "Test@123");
            AssertLoginFailed(login);
        }

        [Test, Description("TC_F1.6_06: Đăng nhập với mật khẩu sai")]
        public void TC_F1_6_06()
        {
            var login = GoToLogin();
            var user = ConfigReader.GetUserData("user");
            login.Login(user.Username, "saimatkhau@123");
            AssertLoginFailed(login);
        }

        [Test, Description("TC_F1.6_07: Đăng nhập với tài khoản bị khóa")]
        public void TC_F1_6_07()
        {
            // Lưu ý: test case này phụ thuộc vào tài khoản bị khóa trên server.
            // Nếu không có tài khoản bị khóa, test sẽ fail — cần setup data trước.
            var login = GoToLogin();
            login.Login("bikhoa@gmail.com", "Test@123");
            AssertLoginFailed(login);
        }

        // ==========================================
        // F1.9: ĐĂNG XUẤT
        // ==========================================

        [Test, Description("TC_F1.9_01: Kiểm tra đăng xuất thành công → chuyển về trang chủ")]
        public void TC_F1_9_01()
        {
            var login = GoToLogin();
            var admin = ConfigReader.GetUserData("admin");
            login.Login(admin.Username, admin.Password);
            Assert.That(login.IsLoginSuccess(), Is.True, "Cần login thành công trước khi test logout");
            login.Logout();
            // Redirect về trang chủ hoặc trang Login đều hợp lệ
            string currentUrl = Driver.Url.ToLower();
            bool isLoggedOut = !currentUrl.Contains("/account/profile")
                            && !currentUrl.Contains("/account/manage");
            Assert.That(isLoggedOut, Is.True, $"Logout thất bại: vẫn còn ở '{Driver.Url}'");
        }

        [Test, Description("TC_F1.9_02: Truy cập trực tiếp URL trang tài khoản sau khi đăng xuất → bị chặn")]
        public void TC_F1_9_02()
        {
            // Bước 1: Đăng nhập
            var login = GoToLogin();
            var admin = ConfigReader.GetUserData("admin");
            login.Login(admin.Username, admin.Password);
            Assert.That(login.IsLoginSuccess(), Is.True);

            // Bước 2: Đăng xuất
            login.Logout();

            // Bước 3: Truy cập trực tiếp URL Profile
            Driver.Navigate().GoToUrl(ProfileUrl);
            System.Threading.Thread.Sleep(2000);

            // Kết quả: phải bị redirect về login hoặc trang chủ, không thể vào Profile
            string afterUrl = Driver.Url.ToLower();
            bool isBlocked = !afterUrl.Contains("/account/profile");
            Assert.That(isBlocked, Is.True, $"Lỗi bảo mật: có thể truy cập Profile sau logout! URL: {Driver.Url}");
        }
    }
}

