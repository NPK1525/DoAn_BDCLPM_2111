using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Collections.Generic;

namespace SeleniumProject.Pages
{
    public class RegisterPage(IWebDriver driver) : BasePage(driver)
    {
        private const string RegisterUrl = "/account/register";
        private const string ErrorSelector = ".text-danger, .alert-danger, .validation-summary-errors, .field-validation-error";
        private const string InvalidSelector = "input:invalid";

        public void Register(string name, string email, string password, string confirm, string phone, string gender)
        {
            FillForm(name, email, password, confirm, phone, gender);
            ClickBtn();
            System.Threading.Thread.Sleep(2000);
        }

        public void ClickCreateAccountOnly() => ClickBtn();

        public void FillForm(string? name, string? email, string? password, string? confirm, string? phone, string? gender)
        {
            if (name != null) Type(By.Name("FullName"), name);
            if (email != null) Type(By.Name("Email"), email);
            if (password != null) Type(By.Id("regPassword"), password);
            if (confirm != null) Type(By.Id("regConfirmPassword"), confirm);
            if (phone != null) Type(By.Name("PhoneNumber"), phone);
            
            if (gender != null) {
                var id = gender.ToLower() == "nam" ? "male" : "female";
                Click(By.Id(id));
            }
        }

        private void ClickBtn()
        {
            var btn = WaitForElementClickable(By.XPath("//button[contains(.,'TẠO')]"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
        }

        public void RegisterWithoutFullName(string email, string password, string confirm, string phone, string gender) => FillForm(null, email, password, confirm, phone, gender);
        public void RegisterWithInvalidEmail(string name, string email, string password, string confirm, string phone, string gender) => Register(name, email, password, confirm, phone, gender);
        public void RegisterWithEmailContainingSpace(string name, string email, string password, string confirm, string phone, string gender) => Register(name, email, password, confirm, phone, gender);
        public void RegisterWithInvalidPhone(string name, string email, string password, string confirm, string phone, string gender) => Register(name, email, password, confirm, phone, gender);

        public void EnterFullName(string name) => Type(By.Name("FullName"), name);
        public void EnterEmail(string email) => Type(By.Name("Email"), email);
        public void EnterPassword(string password) => Type(By.Id("regPassword"), password);
        public void EnterConfirmPassword(string confirm) => Type(By.Id("regConfirmPassword"), confirm);
        public void EnterPhone(string phone) => Type(By.Name("PhoneNumber"), phone);
        public void SelectGender(string gender) => Click(By.Id(gender.ToLower() == "nam" ? "male" : "female"));

        public bool IsRegisterSuccess() => !driver.Url.Contains(RegisterUrl, System.StringComparison.OrdinalIgnoreCase);
        public bool IsStillOnRegisterPage() => driver.Url.Contains(RegisterUrl, System.StringComparison.OrdinalIgnoreCase);
        public bool HasRegisterError() => driver.FindElements(By.CssSelector(ErrorSelector)).Any(e => e.Displayed);
        public string GetRegisterErrors() => string.Join(" | ", driver.FindElements(By.CssSelector(ErrorSelector)).Where(e => e.Displayed).Select(e => e.Text.Trim()));
        public bool HasClientSideValidationError() => driver.FindElements(By.CssSelector(InvalidSelector)).Count > 0;
        public int GetInvalidFieldCount() => driver.FindElements(By.CssSelector(InvalidSelector)).Count;
        public string GetFirstInvalidValidationMessage() => driver.FindElements(By.CssSelector(InvalidSelector)).FirstOrDefault()?.GetAttribute("validationMessage") ?? "";
        public string GetFirstInvalidFieldName() => driver.FindElements(By.CssSelector(InvalidSelector)).FirstOrDefault()?.GetAttribute("name") ?? "";
    }
}
