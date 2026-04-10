using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace SeleniumProject.Pages
{
    public class LoginPage(IWebDriver driver) : BasePage(driver)
    {
        private const string LoginUrl = "/account/login";
        private const string ErrorSelector = "form .text-danger, form .alert-danger, .alert-danger, .validation-summary-errors";

        public void Login(string email, string password)
        {
            var emailBox = WaitForElementVisible(By.Name("Email"));
            emailBox.Clear();
            if (!string.IsNullOrEmpty(email)) emailBox.SendKeys(email);

            var passwordBox = WaitForElementVisible(By.Id("loginPassword"));
            passwordBox.Clear();
            if (!string.IsNullOrEmpty(password)) passwordBox.SendKeys(password);
            
            var btn = WaitForElementClickable(By.XPath("//button[contains(text(),'ĐĂNG')]"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
            System.Threading.Thread.Sleep(2000);
        }

        public void Logout()
        {
            Click(By.CssSelector("a[data-bs-target='#userOffcanvas']"));
            var logoutBtn = WaitForElementClickable(By.CssSelector("#userOffcanvas button.btn-dark"));
            logoutBtn.Click();
            System.Threading.Thread.Sleep(1000);
        }

        public bool IsLoginSuccess() => !driver.Url.Contains(LoginUrl, System.StringComparison.OrdinalIgnoreCase);
        public bool IsStillOnLoginPage() => driver.Url.Contains(LoginUrl, System.StringComparison.OrdinalIgnoreCase);
        public bool HasLoginError() => IsStillOnLoginPage() && driver.FindElements(By.CssSelector(ErrorSelector)).Any(e => e.Displayed);
        public string GetLoginErrors() => string.Join(" | ", driver.FindElements(By.CssSelector(ErrorSelector)).Where(e => e.Displayed).Select(e => e.Text.Trim()));
        
        public bool HasClientSideValidationError() => driver.FindElements(By.CssSelector("input:invalid")).Count > 0;
        public string GetFirstInvalidValidationMessage() => driver.FindElements(By.CssSelector("input:invalid")).FirstOrDefault()?.GetAttribute("validationMessage") ?? "";
        public string GetFirstInvalidFieldName() => driver.FindElements(By.CssSelector("input:invalid")).FirstOrDefault()?.GetAttribute("name") ?? "";
    }
}
