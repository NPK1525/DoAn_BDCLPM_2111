/*
 * Auto Test - F6: Hồ sơ cá nhân (Xem, Cập nhật thông tin, Đổi mật khẩu)
 * Project  : Web bán quần áo - Nhóm 2111
 * Framework: Selenium WebDriver + NUnit
 */

using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumProject.Pages;
using TestProject1.Utilities;

namespace TestProject1.Tests
{
    // ============================================================
    // F6 — Hồ sơ cá nhân (Xem, Cập nhật, Đổi mật khẩu, Điều hướng)
    // ============================================================
    [TestFixture]
    [Category("F6_Profile")]
    public class ProfileTests : BaseTest
    {
        private IWebDriver Driver => driver!;
        private ProfilePage _profile = null!;

        [SetUp]
        public override void Setup()
        {
            driver = DriverFactory.InitDriver();
            
            // Đăng nhập với tài khoản user
            var login = new LoginPage(Driver);
            Driver.Navigate().GoToUrl("https://shop-production-b6d0.up.railway.app/Account/Login");
            var user = ConfigReader.GetUserData("user");
            login.Login(user.Username, user.Password);
            System.Threading.Thread.Sleep(2000);
            
            _profile = new ProfilePage(Driver);
        }

        // ============================================================
        // F6.1 & F6.2 — Truy cập hồ sơ & Cập nhật thông tin cá nhân
        // ============================================================

        [Test, Description("TC_F6.1_01 - Kiểm tra truy cập trang hồ sơ cá nhân")]
        public void TC_F6_1_01_AccessProfilePage()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            Assert.That(_profile.IsProfilePageDisplayed(), Is.True,
                "Không truy cập được trang hồ sơ cá nhân");
        }

        [Test, Description("TC_F6.2_01 - Kiểm tra hiển thị thông tin người dùng")]
        public void TC_F6_2_01_DisplayUserInfo()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            Assert.That(_profile.HasUserInfoSection(), Is.True,
                "Không hiển thị đầy đủ thông tin người dùng (Tên, Email)");
        }

        [Test, Description("TC_F6.2_02 - Kiểm tra trường Email không cho chỉnh sửa")]
        public void TC_F6_2_02_EmailFieldReadOnly()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            Assert.That(_profile.IsEmailReadOnly(), Is.True,
                "Trường Email không ở chế độ readonly");
        }

        [Test, Description("TC_F6.2_03 - Kiểm tra cập nhật họ và tên thành công")]
        public void TC_F6_2_03_UpdateName_Success()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            string newName = "Selenium Test User " + System.DateTime.Now.ToString("HHmmss");
            _profile.UpdateName(newName);
            _profile.ClickSaveProfile();

            bool success = _profile.HasSuccessMessage() || Driver.PageSource.Contains(newName);
            Assert.That(success, Is.True, "Không cập nhật được họ và tên");

            // Khôi phục tên gốc
            _profile.NavigateToDirectly();
            System.Threading.Thread.Sleep(1000);
            _profile.UpdateName("Nguyễn Phúc Khang");
            _profile.ClickSaveProfile();
        }

        [Test, Description("TC_F6.2_04 - Kiểm tra không cho lưu khi để trống họ và tên")]
        public void TC_F6_2_04_UpdateName_Empty_Fail()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            _profile.ClearName();
            _profile.ClickSaveProfile();

            bool hasError = _profile.HasErrorMessage() || _profile.HasValidationError();
            Assert.That(hasError, Is.True, "Không hiển thị lỗi khi để trống họ và tên");
        }

        [Test, Description("TC_F6.2_05 - Kiểm tra cập nhật số điện thoại thành công")]
        public void TC_F6_2_05_UpdatePhone_Success()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            _profile.UpdatePhone("0369876546");
            _profile.ClickSaveProfile();

            bool success = _profile.HasSuccessMessage() || !_profile.HasErrorMessage();
            Assert.That(success, Is.True, "Không cập nhật được số điện thoại");
        }

        [Test, Description("TC_F6.2_06 - Kiểm tra cập nhật số điện thoại để trống")]
        public void TC_F6_2_06_UpdatePhone_Empty_Fail()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            _profile.ClearPhone();
            _profile.ClickSaveProfile();

            bool hasError = _profile.HasErrorMessage() || _profile.HasValidationError();
            Assert.That(hasError, Is.True, "Không hiển thị lỗi khi để trống số điện thoại");
        }

        [Test, Description("TC_F6.2_07 - Kiểm tra cập nhật giới tính")]
        public void TC_F6_2_07_UpdateGender()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            _profile.SelectGender("Nam");
            _profile.ClickSaveProfile();

            bool success = _profile.HasSuccessMessage() || !_profile.HasErrorMessage();
            Assert.That(success, Is.True, "Không cập nhật được giới tính");
        }

        // ============================================================
        // F6.5 — Đổi mật khẩu
        // ============================================================

        [Test, Description("TC_F6.5_01 - Kiểm tra hiển thị form Đổi mật khẩu")]
        public void TC_F6_5_01_ChangePasswordForm_Displayed()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            Assert.That(_profile.IsChangePasswordFormDisplayed(), Is.True,
                "Form đổi mật khẩu không hiển thị");
        }

        [Test, Description("TC_F6.5_03 - Kiểm tra đổi mật khẩu khi sai mật khẩu hiện tại")]
        public void TC_F6_5_03_ChangePassword_WrongCurrentPassword()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            _profile.EnterCurrentPassword("SaiMatKhau@999");
            _profile.EnterNewPassword("NewPass@123");
            _profile.EnterConfirmPassword("NewPass@123");
            _profile.ClickChangePassword();

            bool hasError = _profile.HasErrorMessage() || _profile.HasValidationError();
            Assert.That(hasError, Is.True, "Không hiển thị lỗi khi nhập sai mật khẩu hiện tại");
        }

        [Test, Description("TC_F6.5_04 - Kiểm tra đổi mật khẩu khi xác nhận không khớp")]
        public void TC_F6_5_04_ChangePassword_ConfirmMismatch()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            var user = ConfigReader.GetUserData("user");
            _profile.EnterCurrentPassword(user.Password);
            _profile.EnterNewPassword("NewPass@123");
            _profile.EnterConfirmPassword("KhacMatKhau@456");
            _profile.ClickChangePassword();

            bool hasError = _profile.HasErrorMessage() || _profile.HasValidationError();
            Assert.That(hasError, Is.True, "Không hiển thị lỗi khi xác nhận mật khẩu không khớp");
        }

        [Test, Description("TC_F6.5_05 - Kiểm tra đổi mật khẩu khi để trống các trường")]
        public void TC_F6_5_05_ChangePassword_EmptyFields()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            _profile.EnterCurrentPassword("");
            _profile.EnterNewPassword("");
            _profile.EnterConfirmPassword("");
            _profile.ClickChangePassword();

            bool hasError = _profile.HasErrorMessage() || _profile.HasValidationError();
            Assert.That(hasError, Is.True, "Không hiển thị lỗi khi để trống các trường bắt buộc");
        }

        // ============================================================
        // F6.6 & F6.7 & F6.8 — Điều hướng từ hồ sơ
        // ============================================================

        [Test, Description("TC_F6.6_01 - Kiểm tra điều hướng đến Đơn hàng của tôi")]
        public void TC_F6_6_01_NavigateToOrders()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            _profile.ClickOrdersLink();
            System.Threading.Thread.Sleep(1000);

            Assert.That(Driver.Url, Does.Contain("Order").IgnoreCase,
                "Không điều hướng được đến trang Đơn hàng");
        }

        [Test, Description("TC_F6.7_01 - Kiểm tra điều hướng đến Danh sách yêu thích")]
        public void TC_F6_7_01_NavigateToWishlist()
        {
            _profile.NavigateTo();
            System.Threading.Thread.Sleep(1000);

            _profile.ClickWishlistLink();
            System.Threading.Thread.Sleep(1000);

            Assert.That(Driver.Url, Does.Contain("Wishlist").Or.Contain("wishlist").Or.Contain("Wish"),
                "Không điều hướng được đến trang Danh sách yêu thích");
        }
    }
}
