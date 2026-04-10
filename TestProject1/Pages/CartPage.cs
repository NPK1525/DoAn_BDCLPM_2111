using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Linq;
using SeleniumProject.Pages;

namespace SeleniumProject.Pages
{
    public class CartPage(IWebDriver driver) : BasePage(driver)
    {
        private const string CartUrl = "https://shop-production-b6d0.up.railway.app/Cart";

        public void NavigateTo() => driver.Navigate().GoToUrl(CartUrl);

        private ReadOnlyCollection<IWebElement> GetRows() =>
            driver.FindElements(By.CssSelector(".cart-item, tr.cart-item"));

        public int GetItemCount() => GetRows().Count;
        public int GetCartItemCount() => GetItemCount();
        public bool HasItems() => GetItemCount() > 0;

        public void ClickCheckout()
        {
            // Thử nhiều selector cho nút thanh toán
            var selectors = new[] {
                "//a[contains(text(),'THANH TOÁN')]",
                "//a[contains(text(),'Thanh toán')]",
                "//button[contains(text(),'THANH TOÁN')]",
                "//button[contains(text(),'Thanh toán')]",
                "//a[contains(@href,'Checkout')]",
                "//a[contains(@href,'checkout')]"
            };
            
            IWebElement? btn = null;
            foreach (var selector in selectors) {
                try {
                    btn = driver.FindElement(By.XPath(selector));
                    if (btn.Displayed) {
                        Console.WriteLine($"Found checkout button with selector: {selector}");
                        break;
                    }
                } catch {
                    continue;
                }
            }
            
            if (btn == null) {
                throw new NoSuchElementException("Không tìm thấy nút thanh toán");
            }
            
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
        }

        public string GetCartTotal()
        {
            try {
                var total = driver.FindElement(By.XPath("//div[contains(@class,'card-body')]//h5[contains(text(),'₫')]"));
                return total.Text;
            } catch { 
                try {
                    var bodyText = driver.FindElement(By.TagName("body")).Text;
                    var priceLines = bodyText.Split('\n').Where(line => line.Contains('₫'));
                    return string.Join(" | ", priceLines);
                } catch { return ""; }
            }
        }

        public bool IsItemPriceDisplayed(int index = 0) => GetRows().Count > index && GetRows()[index].Text.Contains('₫');
        public bool IsItemNameDisplayed(int index = 0) => GetRows().Count > index && GetRows()[index].FindElement(By.CssSelector("h6, .product-name")).Displayed;

        public int GetItemQuantity(int index = 0)
        {
            var rows = GetRows();
            if (index >= rows.Count) return 0;
            var input = rows[index].FindElement(By.CssSelector("input.qty-input"));
            return int.TryParse(input.GetAttribute("value"), out int q) ? q : 0;
        }

        public void SetItemQuantity(int index, string qty)
        {
            var rows = GetRows();
            var input = rows[index].FindElement(By.CssSelector("input.qty-input"));
            input.Clear();
            input.SendKeys(qty);
            input.SendKeys(Keys.Enter);
            System.Threading.Thread.Sleep(1500);
        }

        public void ClickIncrease(int index = 0)
        {
            var rows = GetRows();
            try
            {
                var btn = rows[index].FindElement(By.XPath(".//button[contains(text(),'+') or contains(@class,'btn-increase')]"));
                btn.Click();
            }
            catch
            {
                var btns = rows[index].FindElements(By.TagName("button"));
                btns.Last().Click();
            }
            System.Threading.Thread.Sleep(1000);
        }

        public void ClickDecrease(int index = 0)
        {
            var rows = GetRows();
            try
            {
                var btn = rows[index].FindElement(By.XPath(".//button[contains(text(),'-') or contains(@class,'btn-decrease')]"));
                btn.Click();
            }
            catch
            {
                var btns = rows[index].FindElements(By.TagName("button"));
                var decreaseBtn = btns.FirstOrDefault(b => b.Text.Contains('-') || (b.GetAttribute("class")?.Contains("decrease") ?? false));
                if (decreaseBtn != null) decreaseBtn.Click();
                else btns.First().Click();
            }
            System.Threading.Thread.Sleep(1000);
        }

        public void ClickDelete(int index = 0)
        {
            var rows = GetRows();
            var btn = rows[index].FindElement(By.CssSelector(".btn-outline-danger, .fa-trash"));
            btn.Click();
            System.Threading.Thread.Sleep(1500);
        }

        public void ClickClearAll()
        {
            try
            {
                // Thử tìm nút bằng text
                var btn = driver.FindElement(By.XPath("//button[contains(text(),'Làm trống giỏ hàng')]"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
                System.Threading.Thread.Sleep(500);
                btn.Click();
            }
            catch
            {
                // Thử tìm form với action Clear
                var form = driver.FindElement(By.CssSelector("form[action*='Clear']"));
                var btn = form.FindElement(By.CssSelector("button[type='submit']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
                System.Threading.Thread.Sleep(500);
                btn.Click();
            }
            System.Threading.Thread.Sleep(1500);
        }

        public bool IsEmptyMessageDisplayed() => 
            driver.FindElement(By.TagName("body")).Text.Contains("Giỏ hàng trống") || 
            driver.FindElements(By.CssSelector(".text-center h4")).Any(e => e.Text.Contains("trống"));

        public void ClearAllItems()
        {
            NavigateTo();
            while (HasItems())
            {
                ClickDelete(0);
                System.Threading.Thread.Sleep(1500);
                NavigateTo();
            }
        }

        public void EnsureQuantity(int index, int quantity)
        {
            if (HasItems() && index < GetItemCount())
            {
                SetItemQuantity(index, quantity.ToString());
                System.Threading.Thread.Sleep(1500);
                NavigateTo();
            }
        }
    }
}
