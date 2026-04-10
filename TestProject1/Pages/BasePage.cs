using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumProject.Pages
{
    public class BasePage(IWebDriver driver)
    {
        protected readonly IWebDriver driver = driver;
        protected readonly WebDriverWait wait = new(driver, TimeSpan.FromSeconds(15));

        protected IWebElement WaitForElementVisible(By by)
        {
            try
            {
                return wait.Until(ExpectedConditions.ElementIsVisible(by));
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"[ERROR] Element not visible: {by}");
                Console.WriteLine($"Current URL: {driver.Url}");
                throw;
            }
        }

        protected IWebElement WaitForElementClickable(By by)
        {
            try
            {
                return wait.Until(ExpectedConditions.ElementToBeClickable(by));
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"[ERROR] Element not clickable: {by}");
                Console.WriteLine($"Current URL: {driver.Url}");
                throw;
            }
        }

        protected void Click(By by)
        {
            WaitForElementClickable(by).Click();
        }

        protected void Type(By by, string text)
        {
            var element = WaitForElementVisible(by);
            element.Click();
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Delete);

            if (!string.IsNullOrEmpty(text))
                element.SendKeys(text);
        }

        protected string GetText(By by)
        {
            try
            {
                return WaitForElementVisible(by).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        protected bool IsDisplayed(By by, int timeoutSeconds = 3)
        {
            try
            {
                var shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                return shortWait.Until(ExpectedConditions.ElementIsVisible(by)).Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}