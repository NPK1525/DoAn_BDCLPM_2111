using OpenQA.Selenium;
using SeleniumProject.Pages;
using System.Linq;

namespace SeleniumProject.Pages
{
    public class OrderDetailsPage(IWebDriver driver) : BasePage(driver)
    {
        private const string CancelButtonXPath = "//button[contains(.,'Hủy đơn')]";
        private const string ModalConfirmButtonXPath = "(//button[contains(.,'Xác nhận')])[last()]";
        private const string ModalCloseButtonXPath = "//div[@id='cancelModal']//button[@data-bs-dismiss='modal']";

        public void CancelOrder(string reason)
        {
            var btn = WaitForElementClickable(By.XPath(CancelButtonXPath));
            btn.Click();
            Thread.Sleep(1000);

            var area = WaitForElementVisible(By.Id("cancelReason"));
            area.Clear();
            area.SendKeys(reason);
            Thread.Sleep(500);

            // Nút submit trong modal
            var confirmBtn = WaitForElementClickable(By.XPath("//div[@id='cancelModal']//button[@type='submit']"));
            
            try {
                confirmBtn.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", confirmBtn);
            }
            Thread.Sleep(2000);
        }

        public void ClickCancelButtonOnly() => WaitForElementClickable(By.XPath(CancelButtonXPath)).Click();
        
        public void SubmitCancelWithoutReason()
        {
            // Bước 1: Click nút "Hủy đơn" để mở modal
            var btn = WaitForElementClickable(By.XPath(CancelButtonXPath));
            btn.Click();
            Thread.Sleep(1000);
            
            // Bước 2: Không nhập gì vào textarea, chỉ click nút submit
            var confirmBtn = WaitForElementClickable(By.XPath("//div[@id='cancelModal']//button[@type='submit']"));
            
            try {
                confirmBtn.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", confirmBtn);
            }
            Thread.Sleep(1000);
        }
        
        public bool IsCancelModalDisplayed() => IsDisplayed(By.TagName("textarea")) || IsDisplayed(By.XPath("//h5[contains(.,'Lý do hủy đơn')]"));
        
        public void CloseCancelModal()
        {
            try {
                // Thử click nút Đóng bằng JS để tránh bị che bởi backdrop
                var closeBtn = WaitForElementVisible(By.XPath(ModalCloseButtonXPath));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeBtn);
            } catch {
                // Fallback: Nhấn ESC để đóng modal
                driver.FindElement(By.TagName("body")).SendKeys(Keys.Escape);
            }
            Thread.Sleep(500);
        }

        public bool IsCancelButtonDisplayed() => driver.FindElements(By.XPath(CancelButtonXPath)).Any(e => e.Displayed);
        public bool IsConfirmReceivedButtonDisplayed() => driver.FindElements(By.XPath("//button[contains(.,'Đã nhận')]")).Any(e => e.Displayed);

        public void ClickConfirmReceived()
        {
            // Click nút "Đã nhận" để mở modal
            var btn = WaitForElementClickable(By.XPath("//button[contains(.,'Đã nhận')]"));
            btn.Click();
            System.Threading.Thread.Sleep(1000);
            
            // Đợi modal hiển thị
            WaitForElementVisible(By.Id("confirmReceivedModal"));
            System.Threading.Thread.Sleep(500);
            
            // Click nút "Xác nhận đã nhận" trong modal
            var confirmBtn = WaitForElementClickable(By.XPath("//div[@id='confirmReceivedModal']//button[@type='submit']"));
            try {
                confirmBtn.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", confirmBtn);
            }
            
            System.Threading.Thread.Sleep(2000);
        }

        public void CloseConfirmReceivedModal()
        {
            try {
                // Click nút "Chưa nhận" để đóng modal
                var closeBtn = WaitForElementVisible(By.XPath("//div[@id='confirmReceivedModal']//button[@data-bs-dismiss='modal']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeBtn);
            } catch {
                // Fallback: Nhấn ESC để đóng modal
                driver.FindElement(By.TagName("body")).SendKeys(Keys.Escape);
            }
            System.Threading.Thread.Sleep(500);
        }

        public bool IsConfirmReceivedModalDisplayed() => IsDisplayed(By.Id("confirmReceivedModal"));

        // Review methods
        public bool IsReviewButtonDisplayed() => driver.FindElements(By.XPath("//a[contains(@class,'btn-outline-warning') and contains(.,'Đánh giá')]")).Any(e => e.Displayed);

        public void ClickReviewButton()
        {
            // Click vào link "Đánh giá" - sẽ chuyển đến trang sản phẩm với anchor #reviews
            var btn = WaitForElementClickable(By.XPath("(//a[contains(@class,'btn-outline-warning') and contains(.,'Đánh giá')])[1]"));
            
            // Scroll đến element
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
            System.Threading.Thread.Sleep(500);
            
            // Click bằng JavaScript để tránh bị che
            try {
                btn.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
            }
            
            System.Threading.Thread.Sleep(2000); // Đợi chuyển trang và scroll đến #reviews
        }
    }
}
