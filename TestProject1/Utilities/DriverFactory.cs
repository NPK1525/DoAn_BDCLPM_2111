using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace TestProject1.Utilities
{
    public static class DriverFactory
    {
        public static IWebDriver InitDriver()
        {
            ChromeOptions options = new();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            ChromeDriver driver = new(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            return driver;
        }
    }
}