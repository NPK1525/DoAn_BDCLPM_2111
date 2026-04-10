using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TestProject1.Utilities;
using System;
using System.IO;

namespace TestProject1.Tests
{
    public class BaseTest
    {
        protected IWebDriver? driver;

        protected string baseUrl = "https://shop-production-b6d0.up.railway.app";

        [SetUp]
        public virtual void Setup()
        {
            ChromeOptions options = new();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            driver = new ChromeDriver(options);
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            Console.WriteLine("Base URL: " + baseUrl);
        }

        [TearDown]
        public virtual void TearDown()
        {
            string? screenshotPath = null;
            string testName = TestContext.CurrentContext.Test.Name;
            bool testFailed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;

            // Chụp screenshot nếu test failed
            if (testFailed)
            {
                try
                {
                    var screenshot = ((ITakesScreenshot)driver!).GetScreenshot();
                    string dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "Screenshots");
                    Directory.CreateDirectory(dir);
                    string fileName = $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    string filePath = Path.Combine(dir, fileName);
                    screenshot.SaveAsFile(filePath);
                    screenshotPath = filePath;
                    Console.WriteLine("Screenshot saved: " + filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Screenshot failed: " + ex.Message);
                }
            }

            // Ghi kết quả vào Excel
            try
            {
                string status = testFailed ? "Fail" : "Pass";
                ExcelReportHelper.WriteResult(testName, status, screenshotPath ?? "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Excel] Lỗi khi ghi kết quả: {ex.Message}");
            }

            try { Console.WriteLine("Final URL: " + driver?.Url); } catch { }

            driver?.Quit();
            driver?.Dispose();
        }
    }
}
