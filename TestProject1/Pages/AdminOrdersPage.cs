using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumProject.Pages
{
    public class AdminOrdersPage(IWebDriver driver) : BasePage(driver)
    {
        private const string AdminOrdersUrl = "https://shop-production-b6d0.up.railway.app/Admin/Orders";

        public void NavigateTo() => driver.Navigate().GoToUrl(AdminOrdersUrl);

        public void NavigateToDetails(string orderId)
        {
            driver.Navigate().GoToUrl($"{AdminOrdersUrl}/Details/{orderId}");
        }

        public void SearchOrder(string searchText)
        {
            var searchInput = WaitForElementVisible(By.Name("search"));
            searchInput.Clear();
            searchInput.SendKeys(searchText);
            
            var searchBtn = driver.FindElement(By.XPath("//button[@type='submit' and contains(.,'Tìm kiếm')]"));
            searchBtn.Click();
            Thread.Sleep(1000);
        }

        public void FilterByStatus(string status)
        {
            var statusSelect = new SelectElement(driver.FindElement(By.Name("status")));
            statusSelect.SelectByText(status);
            Thread.Sleep(1000);
        }

        public void ClickViewDetails(int index = 0)
        {
            var detailsBtn = driver.FindElements(By.XPath("//a[contains(@class,'btn') and contains(.,'Chi tiết')]"))[index];
            detailsBtn.Click();
            Thread.Sleep(1000);
        }

        public void UpdateOrderStatus(string newStatus)
        {
            var statusSelect = WaitForElementVisible(By.Name("status"));
            var select = new SelectElement(statusSelect);
            select.SelectByText(newStatus);
            
            var updateBtn = driver.FindElement(By.XPath("//button[@type='submit' and contains(.,'Cập nhật')]"));
            updateBtn.Click();
            Thread.Sleep(2000);
        }

        public void CancelOrderAsAdmin(string reason)
        {
            var reasonTextarea = WaitForElementVisible(By.Name("cancelReason"));
            reasonTextarea.Clear();
            reasonTextarea.SendKeys(reason);
            
            var cancelBtn = driver.FindElement(By.XPath("//button[@type='submit' and contains(.,'Xác nhận hủy đơn')]"));
            cancelBtn.Click();
            Thread.Sleep(2000);
        }

        public string GetCurrentOrderStatus()
        {
            try {
                var statusBadge = driver.FindElement(By.XPath("//span[contains(@class,'badge')]"));
                return statusBadge.Text;
            } catch {
                return "";
            }
        }

        public bool IsUpdateStatusFormDisplayed()
        {
            try {
                var form = driver.FindElement(By.XPath("//form[@action='/Admin/Orders/UpdateStatus']"));
                return form.Displayed;
            } catch {
                return false;
            }
        }

        public string GetOrderIdFromDetailsPage()
        {
            try {
                var idElement = driver.FindElement(By.XPath("//h4[contains(.,'#')] | //h5[contains(.,'#')]"));
                var text = idElement.Text;
                if (text.Contains('#')) {
                    return text.Replace("#", "").Trim().Split(' ')[0];
                }
                return "";
            } catch {
                return "";
            }
        }
    }
}
