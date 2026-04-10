using OpenQA.Selenium;

namespace SeleniumProject.Pages
{
    public class HomePage(IWebDriver driver) : BasePage(driver)
    {
        private const string HomeUrl = "https://shop-production-b6d0.up.railway.app/";
        
        private readonly By lnkLogin = By.XPath("//a[contains(.,'Đăng nhập') or contains(.,'Đăng Nhập')]");
        private readonly By lnkRegister = By.XPath("//a[contains(.,'Đăng ký') or contains(.,'Đăng Ký')]");
        private readonly By iconHuman = By.XPath("//a[contains(@class,'dropdown-toggle') or contains(@href,'#')]");
        private readonly By btnLogout = By.XPath("//a[contains(.,'Đăng xuất') or contains(.,'Logout')]");
        private readonly By loggedInIndicator = By.XPath("//*[contains(.,'Đăng xuất') or contains(.,'Tài khoản')]");
        private readonly By iconCart = By.XPath("//a[contains(@href,'cart')] | //i[contains(@class,'fa-shopping-cart')]/..");
        private readonly By firstProduct = By.XPath("(//a[contains(@href,'/Products/Details')] | //div[contains(@class,'product')]//a)[1]");

        public void NavigateTo() => driver.Navigate().GoToUrl(HomeUrl);

        public void GoToLogin()
        {
            Click(lnkLogin);
        }

        public void GoToRegister()
        {
            Click(lnkRegister);
        }

        public bool IsUserLoggedIn()
        {
            return IsDisplayed(loggedInIndicator);
        }

        public void Logout()
        {
            Click(iconHuman);
            Click(btnLogout);
        }

        public void ClickCartIcon()
        {
            var element = WaitForElementVisible(iconCart);
            try {
                element.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
        }

        public void ClickFirstProduct()
        {
            var element = WaitForElementVisible(firstProduct);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);
            System.Threading.Thread.Sleep(500);
            try {
                element.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
            }
        }
    }
}