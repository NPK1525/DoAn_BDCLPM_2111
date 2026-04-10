using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace SeleniumProject.Pages
{
    /// <summary>
    /// Page Object cho trang Danh sách yêu thích (F7)
    /// </summary>
    public class WishlistPage(IWebDriver driver) : BasePage(driver)
    {
        private const string BaseUrl = "https://shop-production-b6d0.up.railway.app";

        // === Điều hướng ===
        public void NavigateTo()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/Wishlist");
            Thread.Sleep(1000);
        }

        public void NavigateViaUserMenu()
        {
            Click(By.CssSelector("a[data-bs-target='#userOffcanvas']"));
            Thread.Sleep(500);
            try
            {
                var link = WaitForElementClickable(By.XPath(
                    "//div[@id='userOffcanvas']//a[contains(.,'Yêu thích') or contains(.,'Wishlist') or contains(@href,'Wishlist')]"));
                link.Click();
            }
            catch
            {
                NavigateTo();
            }
            Thread.Sleep(1000);
        }

        // === Thêm/Xóa yêu thích ===
        public void AddProductToWishlistFromDetails()
        {
            // Bấm icon trái tim trên trang chi tiết sản phẩm
            try
            {
                var heartBtn = WaitForElementClickable(By.CssSelector(
                    ".wishlist-btn, .btn-wishlist, [data-action='wishlist'], " +
                    "button i.fa-heart, a i.fa-heart, .fa-heart"));
                ((IJavaScriptExecutor)driver).ExecuteScript(
                    "arguments[0].scrollIntoView({block:'center'});", heartBtn);
                Thread.Sleep(300);
                try { heartBtn.Click(); }
                catch { ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", heartBtn); }
            }
            catch
            {
                // Thử XPath
                var btn = WaitForElementClickable(By.XPath(
                    "//button[contains(@class,'heart') or contains(@class,'wish')]" +
                    " | //a[contains(@class,'heart') or contains(@class,'wish')]" +
                    " | //*[contains(@class,'fa-heart')]/.."));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
            }
            Thread.Sleep(2000);
        }

        public void RemoveProductFromWishlist(int index = 0)
        {
            try
            {
                // Lấy số lượng sản phẩm ban đầu
                int initialCount = GetProductCount();
                Console.WriteLine($"Số sản phẩm ban đầu: {initialCount}");

                // Thử nhiều selector khác nhau để tìm button xóa
                var selectors = new[]
                {
                    "button.wishlist-btn.active[data-product-id]",  // Button đã active (đã thêm vào wishlist)
                    "button.wishlist-btn[data-product-id]",
                    "button.wishlist-btn.active",
                    "button.wishlist-btn",
                    ".wishlist-btn.active",
                    ".wishlist-btn",
                    "button[data-product-id]",
                    ".product-card button.btn.wishlist-btn",
                    ".card button.wishlist-btn"
                };

                IWebElement? btnToClick = null;
                string? usedSelector = null;

                // Tìm button phù hợp
                foreach (var selector in selectors)
                {
                    try
                    {
                        var btns = driver.FindElements(By.CssSelector(selector));
                        Console.WriteLine($"Selector '{selector}' tìm thấy {btns.Count} elements");
                        
                        if (btns.Count > index)
                        {
                            var btn = btns[index];
                            if (btn.Displayed)
                            {
                                btnToClick = btn;
                                usedSelector = selector;
                                var productId = btn.GetAttribute("data-product-id");
                                Console.WriteLine($"Tìm thấy button với selector '{selector}', product ID: {productId}");
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi với selector '{selector}': {ex.Message}");
                    }
                }

                if (btnToClick == null)
                {
                    Console.WriteLine("Không tìm thấy button xóa nào");
                    return;
                }

                // Click button
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block:'center'});", btnToClick);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnToClick);
                Console.WriteLine($"Đã click vào button với selector '{usedSelector}'");
                Thread.Sleep(1500);
                
                // Kiểm tra alert
                try
                {
                    var alert = driver.SwitchTo().Alert();
                    Console.WriteLine($"Có alert: {alert.Text}");
                    alert.Accept();
                    Thread.Sleep(1000);
                }
                catch { Console.WriteLine("Không có alert"); }
                
                // Kiểm tra modal xác nhận
                try
                {
                    var confirmBtn = driver.FindElement(By.XPath(
                        "//div[contains(@class,'modal')]//button[contains(.,'Xác nhận') or contains(.,'Confirm') or contains(.,'OK') or contains(.,'Yes') or contains(.,'Đồng ý')]"));
                    if (confirmBtn.Displayed)
                    {
                        Console.WriteLine("Tìm thấy modal, click xác nhận");
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", confirmBtn);
                        Thread.Sleep(1500);
                    }
                }
                catch { Console.WriteLine("Không có modal"); }
                
                // Chờ AJAX hoàn thành - kiểm tra URL có thay đổi không
                string currentUrl = driver.Url;
                Console.WriteLine($"URL hiện tại: {currentUrl}");
                
                // Chờ thêm để AJAX hoàn thành
                Thread.Sleep(2000);
                
                // Kiểm tra xem có cần reload không
                int newCount = GetProductCount();
                Console.WriteLine($"Số sản phẩm sau khi click (không reload): {newCount}");
                
                if (newCount >= initialCount)
                {
                    // Nếu count không giảm, thử reload trang
                    Console.WriteLine("Count không giảm, thử reload trang...");
                    driver.Navigate().Refresh();
                    Thread.Sleep(2000);
                    newCount = GetProductCount();
                    Console.WriteLine($"Số sản phẩm sau khi reload: {newCount}");
                }
                
                if (newCount < initialCount)
                {
                    Console.WriteLine("Xóa thành công!");
                }
                else
                {
                    Console.WriteLine("Xóa không thành công - count không giảm");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa sản phẩm khỏi wishlist: {ex.Message}");
            }
        }

        // === Hiển thị danh sách ===
        public int GetProductCount()
        {
            try
            {
                var items = driver.FindElements(By.CssSelector(
                    ".wishlist-item, .product-card, .wish-item, [class*='wishlist'] .card, " +
                    "table tbody tr, .product-item"));
                return items.Count(e => e.Displayed);
            }
            catch { return 0; }
        }

        public bool HasProducts() => GetProductCount() > 0;

        public bool IsEmptyMessageDisplayed()
        {
            var src = driver.PageSource;
            return src.Contains("trống") || src.Contains("không có") || src.Contains("Chưa có") ||
                   src.Contains("empty") || src.Contains("No items") || src.Contains("No products");
        }

        // === Tìm kiếm ===
        public void SearchProduct(string keyword)
        {
            var searchInput = driver.FindElement(By.CssSelector(
                "input[type='search'], input[name*='search'], input[placeholder*='Tìm'], " +
                "input[placeholder*='search'], input[name*='keyword']"));
            searchInput.Click();
            searchInput.SendKeys(Keys.Control + "a");
            searchInput.SendKeys(Keys.Delete);
            searchInput.SendKeys(keyword);
            Thread.Sleep(300);
            // Nhấn Enter hoặc nút tìm
            try
            {
                var btn = driver.FindElement(By.XPath(
                    "//button[contains(.,'Tìm') or @type='submit'] | //input[@type='submit']"));
                btn.Click();
            }
            catch { searchInput.SendKeys(Keys.Enter); }
            Thread.Sleep(2000);
        }

        // === Bộ lọc ===
        public void FilterByCategory(string category)
        {
            try
            {
                // Thử tìm select dropdown trước
                SelectDropdown("Loại sản phẩm", "category", "type", "Category", category);
            }
            catch
            {
                // Nếu không có select, thử tìm button hoặc link với nhiều biến thể
                try
                {
                    // Thử các selector khác nhau
                    var selectors = new[]
                    {
                        $"//button[contains(text(),'{category}')]",
                        $"//a[contains(text(),'{category}')]",
                        $"//button[contains(@value,'{category}')]",
                        $"//a[contains(@href,'category={category}')]",
                        $"//label[contains(text(),'{category}')]/input",
                        $"//input[@value='{category}']",
                        $"//div[contains(@class,'filter')]//button[contains(.,'{category}')]",
                        $"//div[contains(@class,'filter')]//a[contains(.,'{category}')]"
                    };

                    IWebElement? filterElement = null;
                    foreach (var selector in selectors)
                    {
                        try
                        {
                            filterElement = driver.FindElement(By.XPath(selector));
                            if (filterElement.Displayed)
                            {
                                Console.WriteLine($"Tìm thấy filter với selector: {selector}");
                                break;
                            }
                        }
                        catch { }
                    }

                    if (filterElement != null)
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block:'center'});", filterElement);
                        Thread.Sleep(300);
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", filterElement);
                    }
                    else
                    {
                        Console.WriteLine($"Không tìm thấy bộ lọc category cho '{category}' - có thể không có sản phẩm trong danh mục này");
                        throw new NoSuchElementException($"Không tìm thấy bộ lọc category cho '{category}'");
                    }
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine($"Không tìm thấy bộ lọc category cho '{category}'");
                    throw;
                }
            }
            Thread.Sleep(1500);
        }

        public void FilterByGender(string gender)
        {
            try
            {
                SelectDropdown("Giới tính", "gender", "Gender", "sex", gender);
            }
            catch
            {
                // Nếu không có select, thử tìm button hoặc link
                try
                {
                    var filterBtn = driver.FindElement(By.XPath(
                        $"//button[contains(.,'{gender}')] | //a[contains(.,'{gender}')] | " +
                        $"//label[contains(.,'{gender}')]/input | //input[@value='{gender}']"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", filterBtn);
                }
                catch
                {
                    Console.WriteLine($"Không tìm thấy bộ lọc gender cho '{gender}'");
                    throw;
                }
            }
            Thread.Sleep(1500);
        }

        public void SortBy(string sortOption)
        {
            try
            {
                SelectDropdown("Sắp xếp", "sort", "Sort", "orderBy", sortOption);
            }
            catch
            {
                // Nếu không có select, thử tìm button hoặc link
                try
                {
                    var sortBtn = driver.FindElement(By.XPath(
                        $"//button[contains(.,'{sortOption}')] | //a[contains(.,'{sortOption}')]"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", sortBtn);
                }
                catch
                {
                    Console.WriteLine($"Không tìm thấy tùy chọn sắp xếp cho '{sortOption}'");
                    throw;
                }
            }
            Thread.Sleep(1500);
        }

        public void ClearFilters()
        {
            try
            {
                var btn = WaitForElementClickable(By.XPath(
                    "//button[contains(.,'Xóa bộ lọc') or contains(.,'Reset') or contains(.,'Xóa lọc')]" +
                    " | //a[contains(.,'Xóa bộ lọc') or contains(.,'Reset')]"));
                btn.Click();
            }
            catch
            {
                // Reload trang
                NavigateTo();
            }
            Thread.Sleep(1500);
        }

        public void ClickFilterButton()
        {
            try
            {
                var btn = WaitForElementClickable(By.XPath(
                    "//button[contains(.,'Lọc') or contains(.,'Filter') or @type='submit']"));
                btn.Click();
                Thread.Sleep(1500);
            }
            catch { }
        }

        // === Helper ===
        private void SelectDropdown(string label, params string[] hints)
        {
            string value = hints.Last();
            var selectHints = hints.Take(hints.Length - 1).ToArray();

            IWebElement? selectEl = null;
            
            // Thử tìm theo name hoặc id
            foreach (var hint in selectHints)
            {
                try
                {
                    selectEl = driver.FindElement(By.CssSelector(
                        $"select[name*='{hint}'], select[id*='{hint}']"));
                    Console.WriteLine($"Tìm thấy select với hint: {hint}");
                    break;
                }
                catch { }
            }

            // Thử tìm theo label
            if (selectEl == null)
            {
                try
                {
                    selectEl = driver.FindElement(By.XPath(
                        $"//label[contains(.,'{label}')]/following::select[1]"));
                    Console.WriteLine($"Tìm thấy select theo label: {label}");
                }
                catch { }
            }

            // Thử tìm theo class
            if (selectEl == null && selectHints.Length > 0)
            {
                try
                {
                    selectEl = driver.FindElement(By.XPath(
                        $"//select[contains(@class,'{selectHints[0]}')]"));
                    Console.WriteLine($"Tìm thấy select theo class: {selectHints[0]}");
                }
                catch { }
            }

            if (selectEl == null)
            {
                Console.WriteLine($"Không tìm thấy select dropdown cho label '{label}' với hints: {string.Join(", ", selectHints)}");
                throw new NoSuchElementException($"Không tìm thấy select dropdown cho '{label}'");
            }

            var select = new SelectElement(selectEl);
            try 
            { 
                select.SelectByText(value);
                Console.WriteLine($"Đã chọn option theo text: {value}");
            }
            catch 
            { 
                select.SelectByValue(value);
                Console.WriteLine($"Đã chọn option theo value: {value}");
            }
        }

        public string GetFirstProductName()
        {
            try
            {
                var el = driver.FindElement(By.CssSelector(
                    ".wishlist-item .product-name, .product-card .card-title, " +
                    ".wish-item .name, .product-item h5, .product-item .title"));
                return el.Text.Trim();
            }
            catch { return string.Empty; }
        }
    }
}
