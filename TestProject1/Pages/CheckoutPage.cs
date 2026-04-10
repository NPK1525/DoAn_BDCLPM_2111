using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumProject.Pages
{
    public class CheckoutPage(IWebDriver driver) : BasePage(driver)
    {
        private readonly By ddlProvince = By.Id("provinceSelect");
        private readonly By ddlDistrict = By.Id("districtSelect");
        private readonly By ddlWard = By.Id("wardSelect");
        private readonly By txtAddressDetail = By.Id("addressDetail");
        private readonly By txtFullName = By.Id("fullName");
        private readonly By txtPhone = By.Id("phoneNumber");

        public void SelectAddress()
        {
            wait.Until(d => d.FindElements(By.XPath("//select[@id='provinceSelect']/option")).Count > 1);
            new SelectElement(driver.FindElement(ddlProvince)).SelectByIndex(1);

            wait.Until(d => d.FindElements(By.XPath("//select[@id='districtSelect']/option")).Count > 1);
            new SelectElement(driver.FindElement(ddlDistrict)).SelectByIndex(1);

            wait.Until(d => d.FindElements(By.XPath("//select[@id='wardSelect']/option")).Count > 1);
            new SelectElement(driver.FindElement(ddlWard)).SelectByIndex(1);
        }

        public void EnterAddress(string address)
        {
            var addr = wait.Until(ExpectedConditions.ElementIsVisible(txtAddressDetail));
            addr.Clear();
            addr.SendKeys(address);
        }

        public void SubmitOrder()
        {
            ClickSubmitOrder();

            // Đợi trang success load xong (quan trọng)
            wait.Until(d => d.Url.Contains("OrderSuccess"));

            // Đợi text hiển thị
            wait.Until(d => d.PageSource.Contains("Đặt hàng thành công!"));

            Console.WriteLine("Đặt hàng thành công!");
        }

        public void ClickSubmitOrder()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // Button clickable
            var btn = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath("//button[@form='checkoutForm'] | //button[contains(text(), 'Thanh toán')]")));

            Console.WriteLine("Found button: " + btn.Text);

            // Click an toàn
            try
            {
                btn.Click();
            }
            catch
            {
                js.ExecuteScript("arguments[0].click();", btn);
            }

            // Handle alert
            try
            {
                wait.Until(ExpectedConditions.AlertIsPresent());

                var alert = driver.SwitchTo().Alert();
                Console.WriteLine("Alert: " + alert.Text);

                alert.Accept();
                Console.WriteLine("Confirmed");
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Không có alert");
            }
        }

        public void ClearAddress()
        {
            var addr = wait.Until(ExpectedConditions.ElementIsVisible(txtAddressDetail));
            addr.Clear();
        }

        public void EnterPhoneNumber(string phone)
        {
            var phoneInput = wait.Until(ExpectedConditions.ElementIsVisible(txtPhone));
            phoneInput.Clear();
            phoneInput.SendKeys(phone);
        }

        public void ClearForm()
        {
            ClearAddress();
            try {
                var phoneInput = driver.FindElement(txtPhone);
                phoneInput.Clear();
            } catch (NoSuchElementException) { }
            
            try {
                var nameInput = driver.FindElement(txtFullName);
                nameInput.Clear();
            } catch (NoSuchElementException) { }
        }

        public void ClickCancel()
        {
            try
            {
                var cancelBtn = wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.XPath("//button[contains(text(), 'Hủy')] | //a[contains(text(), 'Hủy')] | //a[contains(@href, 'cart')]")));
                cancelBtn.Click();
            }
            catch
            {
                driver.Navigate().Back();
            }
        }
    }
}
