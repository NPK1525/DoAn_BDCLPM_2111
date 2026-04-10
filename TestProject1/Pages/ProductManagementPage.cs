using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using SeleniumProject.Pages;

namespace TestProject1.Pages;

/// <summary>
/// Page Object cho trang Quản lý Sản phẩm phía Admin (/Admin/Products).
/// Phục vụ F9 — Quản lý sản phẩm, và một số test cross-page của F10.
/// </summary>
public class ProductManagementPage(IWebDriver driver) : BasePage(driver)
{
    private const string BaseUrl = "https://shop-production-b6d0.up.railway.app";

    // ===== Locators - Listing =====
    private static readonly By TableRows = By.CssSelector(".table tbody tr");
    private static readonly By TableHeaders = By.CssSelector(".table thead th");
    private static readonly By AddProductLink = By.CssSelector("a[href*='/Admin/Products/Create']");
    private static readonly By EditLinks = By.CssSelector("a[href*='/Admin/Products/Edit/']");
    private static readonly By FirstProductNameCell = By.CssSelector(".table tbody tr:first-child td:nth-child(2)");

    // ===== Locators - Search/Filter =====
    private static readonly By SearchInput = By.Name("search");
    private static readonly By CategoryFilterDropdown = By.Name("category");
    private static readonly By GenderFilterDropdown = By.Name("gender");
    private static readonly By SortByDropdown = By.Name("sortBy");
    private static readonly By FilterSubmitBtn = By.CssSelector("button[type='submit'].btn-primary");
    private static readonly By NotFoundXPath = By.XPath(
        "//*[contains(text(),'không tìm thấy') or contains(text(),'Không có') or contains(text(),'không có')]");

    // ===== Locators - Form Create/Edit =====
    private static readonly By NameInput = By.Id("Name");
    private static readonly By DescriptionInput = By.Id("Description");
    private static readonly By CategoryDropdown = By.Id("Category");
    private static readonly By GenderDropdown = By.Id("Gender");
    private static readonly By ImageFileInput = By.Name("imageFile");
    private static readonly By AdditionalImagesInput = By.Name("additionalImages");
    private static readonly By BackButton = By.CssSelector("a.btn-secondary[href*='/Admin/Products']");

    // ===== Locators - Delete actions =====
    private static readonly By DeleteFormBtn = By.CssSelector(
        "form[action*='/Admin/Products/Delete/'] button[type='submit']");
    private static readonly By PermanentDeleteFormBtn = By.CssSelector(
        "form[action*='/Admin/Products/PermanentDelete/'] button[type='submit']");
    private static readonly By RestoreFormBtn = By.CssSelector(
        "form[action*='/Admin/Products/Restore/'] button[type='submit']");

    // ===== Locators - Stats / Badges =====
    private static readonly By PrimaryStatCard = By.CssSelector(
        ".card.bg-primary .card-body, .card .card-body");
    private static readonly By AllStatCards = By.CssSelector(".card");
    private static readonly By PrimaryTotalNumber = By.CssSelector(
        ".card.bg-primary .card-body h3, .card.bg-primary .card-body .h3");
    private static readonly By StockBadges = By.CssSelector(
        ".badge.bg-success, .badge.bg-danger, .badge.bg-warning");

    // ===== Navigation =====

    public void NavigateToList()
    {
        driver.Navigate().GoToUrl($"{BaseUrl}/Admin/Products");
        WaitForPageReady();
    }

    public void NavigateToCreate()
    {
        driver.Navigate().GoToUrl($"{BaseUrl}/Admin/Products/Create");
        WaitForPageReady();
    }

    public void NavigateToDeleted()
    {
        driver.Navigate().GoToUrl($"{BaseUrl}/Admin/Products/Deleted");
        WaitForPageReady();
    }

    public void ClickAddProductButton()
    {
        WaitForElementClickable(AddProductLink).Click();
        WaitForPageReady();
    }

    // ===== Listing - rows =====

    public int GetTableRowCount() => driver.FindElements(TableRows).Count;

    public List<string> GetTableHeaderTexts() =>
        [.. driver.FindElements(TableHeaders).Select(h => h.Text.Trim())];

    public string GetFirstProductName() =>
        driver.FindElement(FirstProductNameCell).Text.Trim();

    public string GetProductNameInRow(int rowIndex)
    {
        var rows = driver.FindElements(TableRows);
        return rows[rowIndex].FindElement(By.CssSelector("td:nth-child(2)")).Text.Trim();
    }

    public void ClickEditButtonInRow(int rowIndex)
    {
        var rows = driver.FindElements(TableRows);
        rows[rowIndex].FindElement(EditLinks).Click();
        WaitForPageReady();
    }

    public void ClickFirstEditButton()
    {
        WaitForElementClickable(EditLinks).Click();
        WaitForPageReady();
    }

    // ===== Search / Filter =====

    public void EnterSearchKeyword(string keyword)
    {
        var input = WaitForElementVisible(SearchInput);
        input.Clear();
        input.SendKeys(keyword);
    }

    public void SelectCategoryFilterByText(string text)
    {
        SmartSelectByText(CategoryFilterDropdown, text);
    }

    public void SelectGenderFilterByText(string text)
    {
        new SelectElement(WaitForElementVisible(GenderFilterDropdown)).SelectByText(text);
    }

    public void SelectSortByIndex(int index)
    {
        new SelectElement(driver.FindElement(SortByDropdown)).SelectByIndex(index);
    }

    public void ClickFilterSubmit()
    {
        driver.FindElement(FilterSubmitBtn).Click();
        WaitForPageReady();
    }

    public bool HasNotFoundMessage() => IsDisplayed(NotFoundXPath, 2);

    // ===== Form Create/Edit =====

    public void TypeProductName(string name)
    {
        var input = WaitForElementVisible(NameInput);
        input.Clear();
        input.SendKeys(name);
    }

    public void ClearProductName() => WaitForElementVisible(NameInput).Clear();

    public void AppendProductName(string name) => WaitForElementVisible(NameInput).SendKeys(name);

    public void TypeProductDescription(string desc)
    {
        var input = WaitForElementVisible(DescriptionInput);
        input.Clear();
        input.SendKeys(desc);
    }

    /// <summary>Lấy danh sách option text trong dropdown Danh mục</summary>
    public List<string> GetCategoryDropdownOptions()
    {
        var select = new SelectElement(WaitForElementVisible(CategoryDropdown));
        return [.. select.Options.Select(o => o.Text)];
    }

    public int GetCategoryDropdownOptionCount() => GetCategoryDropdownOptions().Count;

    public void SelectCategoryByText(string text)
    {
        SmartSelectByText(CategoryDropdown, text);
    }

    public void SelectCategoryByIndex(int index)
    {
        new SelectElement(driver.FindElement(CategoryDropdown)).SelectByIndex(index);
    }

    public string GetSelectedCategoryText() =>
        new SelectElement(WaitForElementVisible(CategoryDropdown)).SelectedOption.Text;

    /// <summary>Lấy các option (text) của dropdown Category mà khác current và có value hợp lệ</summary>
    public List<string> GetOtherValidCategoryOptions(string currentText)
    {
        var select = new SelectElement(WaitForElementVisible(CategoryDropdown));
        return [.. select.Options
            .Where(o => o.Text != currentText && !string.IsNullOrWhiteSpace(o.GetDomAttribute("value")))
            .Select(o => o.Text)];
    }

    public void SelectGenderByText(string text)
    {
        new SelectElement(WaitForElementVisible(GenderDropdown)).SelectByText(text);
    }

    public void SelectGenderByIndex(int index)
    {
        new SelectElement(driver.FindElement(GenderDropdown)).SelectByIndex(index);
    }

    /// <summary>Tick size checkbox theo Id (vd "sizeM", "sizeXXL")</summary>
    public void TickSizeById(string id)
    {
        var cb = driver.FindElement(By.Id(id));
        if (!cb.Selected) cb.Click();
    }

    /// <summary>Bỏ tick size checkbox theo Id</summary>
    public void UntickSizeById(string id)
    {
        try
        {
            var cb = driver.FindElement(By.Id(id));
            if (cb.Selected) cb.Click();
        }
        catch { }
    }

    /// <summary>Bỏ tick TẤT CẢ size checkbox</summary>
    public void UntickAllSizes()
    {
        string[] sizeIds = ["sizeS", "sizeM", "sizeL", "sizeXL", "sizeXXL",
            "sizeXXXL", "sizeFreesize", "sizeNone"];
        foreach (var id in sizeIds) UntickSizeById(id);
    }

    /// <summary>Tick color checkbox theo Id (vd "colorBlack", "colorWhite")</summary>
    public void TickColorById(string id)
    {
        try
        {
            var cb = driver.FindElement(By.Id(id));
            if (!cb.Selected) cb.Click();
        }
        catch (NoSuchElementException)
        {
            // Nếu không tìm thấy theo ID, thử tìm theo label text
            Console.WriteLine($"Không tìm thấy checkbox với ID '{id}', thử tìm theo label text");
            
            // Lấy tên màu từ ID (bỏ prefix "color")
            string colorName = id.Replace("color", "");
            
            try
            {
                // Thử tìm checkbox theo label text
                var checkbox = driver.FindElement(By.XPath(
                    $"//label[contains(text(),'{colorName}')]/input[@type='checkbox'] | " +
                    $"//input[@type='checkbox' and @value='{colorName}'] | " +
                    $"//input[@type='checkbox' and contains(@name,'color') and @value='{colorName}']"));
                
                if (!checkbox.Selected)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", checkbox);
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"Không tìm thấy checkbox màu '{colorName}' bằng bất kỳ phương pháp nào");
                throw new NoSuchElementException($"Không tìm thấy checkbox màu '{colorName}' (ID: {id})");
            }
        }
    }

    public void UploadMainImage(string filePath)
    {
        driver.FindElement(ImageFileInput).SendKeys(filePath);
    }

    public void UploadAdditionalImages(IEnumerable<string> filePaths)
    {
        driver.FindElement(AdditionalImagesInput).SendKeys(string.Join("\n", filePaths));
    }

    public void ClickBackButton()
    {
        driver.FindElement(BackButton).Click();
        WaitForPageReady();
    }

    // ===== Delete / Permanent Delete / Restore =====

    /// <summary>Nhấn nút xóa SP đầu tiên trong DS</summary>
    public bool ClickFirstDeleteButton()
    {
        var btns = driver.FindElements(DeleteFormBtn);
        if (btns.Count == 0) return false;
        ScrollIntoView(btns[0]);
        btns[0].Click();
        return true;
    }

    public int GetDeleteFormCount() => driver.FindElements(DeleteFormBtn).Count;

    public bool ClickFirstPermanentDeleteButton()
    {
        var btns = driver.FindElements(PermanentDeleteFormBtn);
        if (btns.Count == 0) return false;
        ScrollIntoView(btns[0]);
        btns[0].Click();
        return true;
    }

    public bool ClickFirstRestoreButton()
    {
        var btns = driver.FindElements(RestoreFormBtn);
        if (btns.Count == 0) return false;
        ScrollIntoView(btns[0]);
        btns[0].Click();
        return true;
    }

    /// <summary>Xử lý alert: accept hoặc dismiss. Trả về true nếu có alert.</summary>
    public bool HandleAlert(bool accept)
    {
        try
        {
            var alert = wait.Until(ExpectedConditions.AlertIsPresent());
            if (accept) alert.Accept(); else alert.Dismiss();
            return true;
        }
        catch (WebDriverTimeoutException) { return false; }
    }

    // ===== Stats / Badges =====

    public int CountStatsCards() => driver.FindElements(AllStatCards).Count;

    public int CountPrimaryStatCards() => driver.FindElements(PrimaryStatCard).Count;

    /// <summary>Lấy text số tổng SP từ card .bg-primary (rỗng nếu không tìm thấy)</summary>
    public string GetTotalProductsText()
    {
        var els = driver.FindElements(PrimaryTotalNumber);
        return els.Count > 0 ? els[0].Text.Trim() : "";
    }

    public int CountStockBadges() => driver.FindElements(StockBadges).Count;

    public List<IWebElement> GetStockBadges() => [.. driver.FindElements(StockBadges)];

    // ===== Helpers =====

    /// <summary>
    /// Chọn option dropdown theo thứ tự ưu tiên: SelectByText → SelectByValue →
    /// partial match (case-insensitive). Throw nếu không tìm thấy phù hợp.
    /// </summary>
    private void SmartSelectByText(By dropdownLocator, string target)
    {
        var select = new SelectElement(WaitForElementVisible(dropdownLocator));

        // 1) Exact text match
        try { select.SelectByText(target); return; }
        catch (NoSuchElementException) { }

        // 2) Exact value match
        try { select.SelectByValue(target); return; }
        catch (NoSuchElementException) { }

        // 3) Partial text match (case-insensitive)
        var match = select.Options.FirstOrDefault(o =>
            o.Text.Contains(target, StringComparison.OrdinalIgnoreCase) ||
            (o.GetDomAttribute("value") ?? "").Contains(target, StringComparison.OrdinalIgnoreCase));

        if (match != null)
        {
            select.SelectByText(match.Text);
            return;
        }

        var available = string.Join(", ", select.Options.Select(o =>
            $"'{o.Text}' (value='{o.GetDomAttribute("value")}')"));
        throw new NoSuchElementException(
            $"Không tìm thấy option '{target}' trong dropdown. Options có sẵn: {available}");
    }

    private void ScrollIntoView(IWebElement el)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript(
            "arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", el);
        Thread.Sleep(300);
    }

    private void WaitForPageReady()
    {
        wait.Until(d => ((IJavaScriptExecutor)d)
            .ExecuteScript("return document.readyState")?.ToString() == "complete");
    }
}
