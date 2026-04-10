using OpenQA.Selenium;
using SeleniumProject.Pages;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumProject.Pages
{
    public class OrdersPage(IWebDriver driver) : BasePage(driver)
    {
        private const string OrdersUrl = "https://shop-production-b6d0.up.railway.app/Orders";

        public void NavigateTo() => driver.Navigate().GoToUrl(OrdersUrl);

        // Tab selection logic (Chăm sóc 3 Tab chính hiện có trên Production)
        public void SelectProcessingTab() => Click(By.Id("processing-tab"));
        public void SelectDeliveredTab() => Click(By.Id("completed-tab"));
        public void SelectCancelledTab() => Click(By.Id("cancelled-tab"));

        // Logic đếm đơn hàng trong tab đang active
        public int GetCurrentTabOrderCount()
        {
            try {
                System.Threading.Thread.Sleep(1000); // Đợi tab load
                
                // Đếm theo row/item chính (direct children của tab-pane)
                var orderRows = By.XPath("//div[contains(@class,'tab-pane') and contains(@class,'active')]/div[contains(@class,'mb-') or contains(@class,'order-item') or contains(@class,'card')]");
                var count = driver.FindElements(orderRows).Count(e => e.Displayed);
                
                return count;
            } catch (Exception ex) { 
                Console.WriteLine($"Error in GetCurrentTabOrderCount: {ex.Message}");
                return 0; 
            }
        }

        public bool IsEmptyMessageDisplayed()
        {
            try {
                var bodyText = driver.FindElement(By.TagName("body")).Text;
                return bodyText.Contains("Bạn chưa có đơn hàng nào") ||
                       bodyText.Contains("Không có đơn hàng đang xử lý") ||
                       bodyText.Contains("Chưa có đơn hàng nào hoàn thành") ||
                       bodyText.Contains("Không có đơn hàng bị hủy");
            } catch {
                return false;
            }
        }

        public void ClickViewDetails(int index = 0)
        {
            // Nhấn vào nút 'Xem chi tiết' tương ứng với đơn hàng có dấu # (index)
            var btnLocator = By.XPath("(//div[contains(@class,'tab-pane') and contains(@class,'active')]//a[contains(.,'Xem chi tiết')])[" + (index + 1) + "]");
            Click(btnLocator);
        }

        public string GetFirstOrderId()
        {
            try {
                // Selector chính xác dựa trên HTML structure
                var idElement = driver.FindElement(By.XPath("(//div[contains(@class,'tab-pane') and contains(@class,'active')]//p[contains(@class,'fw-bold') and contains(.,'#')])[1]"));
                var text = idElement.Text; // "#000020"
                Console.WriteLine($"Found order ID text: {text}");
                
                // Extract số từ text "#000020" -> "000020"
                if (text.Contains('#')) {
                    var id = text.Replace("#", "").Trim();
                    return id;
                }
                
                throw new NoSuchElementException("Order ID không có ký tự #");
            } catch (Exception ex) {
                Console.WriteLine($"Error in GetFirstOrderId: {ex.Message}");
                throw;
            }
        }
    }
}
