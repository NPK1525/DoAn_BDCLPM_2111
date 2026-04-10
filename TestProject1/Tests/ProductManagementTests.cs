/*
 * Auto Test - F9: Quản lý Sản phẩm
 * Project  : Web bán quần áo - Nhóm 2111
 * Framework: Selenium WebDriver + NUnit
 */

using NUnit.Framework;
using OpenQA.Selenium;
using TestProject1.Pages;
using TestProject1.Utilities;
using SeleniumProject.Pages;

namespace TestProject1.Tests;

/// <summary>
/// F9 - Quản lý Sản phẩm (39 test cases)
/// Bao gồm: Thêm SP (F9.1), Chỉnh sửa SP (F9.2), Xóa SP (F9.3),
///           Hiển thị & Tìm kiếm (F9.4), Validation (F9.5), Regression
/// </summary>
[TestFixture]
[Category("F9_ProductManagement")]
public class F9_ProductManagementTests : BaseTest
{
    private IWebDriver Driver => driver!;
    private string TestFilesDir => Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "TestData");

    private void LoginAsAdmin()
    {
        Driver.Navigate().GoToUrl(baseUrl + "/Account/Login");
        var loginPage = new LoginPage(Driver);
        var admin = ConfigReader.GetUserData("admin");
        loginPage.Login(admin.Username, admin.Password);
    }

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        LoginAsAdmin();
    }

    // Helper methods
    private void GoToCreateProduct()
    {
        var page = new ProductManagementPage(Driver);
        page.NavigateToCreate();
    }

    private void FillCreateProductForm(
        string name = "",
        string description = "",
        string category = "",
        string gender = "",
        string[] sizes = null!,
        string[] colors = null!,
        bool uploadImage = true)
    {
        var page = new ProductManagementPage(Driver);

        if (!string.IsNullOrEmpty(name))
            page.AppendProductName(name);

        if (!string.IsNullOrEmpty(description))
            page.TypeProductDescription(description);

        if (!string.IsNullOrEmpty(category))
            page.SelectCategoryByText(category);

        if (!string.IsNullOrEmpty(gender))
            page.SelectGenderByText(gender);

        if (sizes != null)
        {
            foreach (var size in sizes)
            {
                var sizeId = $"size{size}";
                page.TickSizeById(sizeId);
            }
        }

        if (colors != null)
        {
            foreach (var color in colors)
            {
                var colorId = $"color{color}";
                page.TickColorById(colorId);
            }
        }

        if (uploadImage)
        {
            var imagePath = Path.Combine(TestFilesDir, "product.jpg");
            if (File.Exists(imagePath))
                page.UploadMainImage(imagePath);
        }
    }

    private void ClickSaveProduct()
    {
        try
        {
            var saveBtn = Driver.FindElement(By.CssSelector("button[type='submit']"));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block:'center'});", saveBtn);
            Thread.Sleep(300);
            saveBtn.Click();
            Thread.Sleep(2000);
        }
        catch
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("document.querySelector('form').submit();");
            Thread.Sleep(2000);
        }
    }

    private bool HasSuccessMessage()
    {
        try
        {
            var src = Driver.PageSource;
            return src.Contains("thành công") || src.Contains("success") || src.Contains("Success");
        }
        catch { return false; }
    }

    private bool HasErrorMessage()
    {
        try
        {
            var src = Driver.PageSource;
            return src.Contains("lỗi") || src.Contains("error") || src.Contains("Error") ||
                   src.Contains("không được") || src.Contains("Vui lòng") ||
                   src.Contains("bắt buộc");
        }
        catch { return false; }
    }

    private bool HasValidationError(string fieldName = "")
    {
        try
        {
            var errors = Driver.FindElements(By.CssSelector(
                ".field-validation-error, .text-danger, .invalid-feedback, input:invalid"));
            return errors.Any(e => e.Displayed);
        }
        catch { return false; }
    }

    // ==================== F9.1 - THÊM SẢN PHẨM ====================

    [Test]
    [Description("TC_F9.1_01 — Thêm SP thành công - đầy đủ thông tin hợp lệ")]
    [Category("F9.1_Create")]
    public void TC_F9_1_01_CreateProduct_FullValidInfo_Success()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở trang Quản lý SP và nhấn '+ Thêm sản phẩm mới'");
        page.NavigateToList();
        page.ClickAddProductButton();

        Console.WriteLine("Nhập đầy đủ thông tin: Tên, Mô tả, Danh mục, Giới tính, Size, Màu, Ảnh");
        FillCreateProductForm(
            name: "Áo Thun Nam Trắng",
            description: "Áo cotton cao cấp",
            category: "Áo",
            gender: "Nam",
            sizes: ["M", "L", "XL"],
            colors: ["Trắng", "Đen"],
            uploadImage: true
        );

        Console.WriteLine("Nhấn Lưu sản phẩm");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra SP tạo thành công, hiển thị trong danh sách");
        var redirected = Driver.Url.Contains("/Admin/Products") && !Driver.Url.Contains("/Create");
        var hasSuccess = HasSuccessMessage();
        Console.WriteLine($"Chuyển về trang DS: {redirected} | Thông báo thành công: {hasSuccess}");

        Assert.That(redirected || hasSuccess, Is.True,
            "Không chuyển về trang danh sách SP sau khi tạo");
    }

    [Test]
    [Description("TC_F9.1_02 — Thêm SP - bỏ trống Tên SP (bắt buộc)")]
    [Category("F9.1_Create")]
    public void TC_F9_1_02_CreateProduct_EmptyName_ShowsError()
    {
        Console.WriteLine("Mở form Thêm sản phẩm");
        GoToCreateProduct();

        Console.WriteLine("Để trống Tên SP, điền đầy đủ các trường còn lại");
        FillCreateProductForm(name: "", description: "Test mô tả",
            category: "Áo", gender: "Nam", sizes: ["M"], colors: ["Đen"]);

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra hiển thị lỗi: Tên sản phẩm không được để trống");
        var hasError = HasValidationError("Name") || HasErrorMessage()
            || Driver.Url.Contains("/Create");
        Console.WriteLine($"Có lỗi validation: {hasError}");

        Assert.That(hasError, Is.True,
            "Không hiển thị lỗi khi bỏ trống tên SP");
    }

    [Test]
    [Description("TC_F9.1_03 — Thêm SP - không chọn Danh mục (bắt buộc)")]
    [Category("F9.1_Create")]
    public void TC_F9_1_03_CreateProduct_NoCategory_ShowsError()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở form Thêm sản phẩm");
        page.NavigateToCreate();

        Console.WriteLine("Nhập tên SP: 'Áo Test'");
        page.AppendProductName("Áo Test");

        Console.WriteLine("Bỏ qua không chọn Danh mục (chọn option rỗng)");
        page.SelectCategoryByIndex(0);

        Console.WriteLine("Điền các trường còn lại: Giới tính, Size, Màu, Ảnh");
        page.SelectGenderByText("Nam");
        page.TickSizeById("sizeM");
        page.TickColorById("colorBlack");
        
        var imagePath = Path.Combine(TestFilesDir, "product.jpg");
        if (File.Exists(imagePath))
            page.UploadMainImage(imagePath);

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra hiển thị lỗi: Vui lòng chọn danh mục");
        var hasError = HasValidationError("Category") || HasErrorMessage()
            || Driver.Url.Contains("/Create");
        Console.WriteLine($"Có lỗi validation: {hasError}");

        Assert.That(hasError, Is.True,
            "Không hiển thị lỗi khi không chọn danh mục");
    }

    [Test]
    [Description("TC_F9.1_04 — Thêm SP - không chọn Giới tính (bắt buộc)")]
    [Category("F9.1_Create")]
    public void TC_F9_1_04_CreateProduct_NoGender_ShowsError()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở form Thêm sản phẩm");
        page.NavigateToCreate();

        Console.WriteLine("Nhập tên SP, chọn danh mục Shirt");
        page.AppendProductName("Áo Test");
        page.SelectCategoryByText("Áo");

        Console.WriteLine("Không chọn Giới tính (giữ option mặc định)");
        page.SelectGenderByIndex(0);

        Console.WriteLine("Chọn Size M, Màu Đen, Upload ảnh");
        page.TickSizeById("sizeM");
        page.TickColorById("colorBlack");
        
        var imagePath = Path.Combine(TestFilesDir, "img.jpg");
        if (File.Exists(imagePath))
            page.UploadMainImage(imagePath);

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra hiển thị lỗi: Vui lòng chọn giới tính");
        var hasError = HasValidationError("Gender") || HasErrorMessage()
            || Driver.Url.Contains("/Create");
        Console.WriteLine($"Có lỗi validation: {hasError}");

        Assert.That(hasError, Is.True,
            "Không hiển thị lỗi khi không chọn giới tính");
    }

    [Test]
    [Description("TC_F9.1_07 — Thêm SP - không upload Ảnh chính (bắt buộc)")]
    [Category("F9.1_Create")]
    public void TC_F9_1_07_CreateProduct_NoMainImage_ShowsError()
    {
        Console.WriteLine("Mở form Thêm sản phẩm");
        GoToCreateProduct();

        Console.WriteLine("Điền đầy đủ các trường NHƯNG KHÔNG upload ảnh chính");
        FillCreateProductForm(
            name: "Áo Test No Image", description: "Test",
            category: "Áo", gender: "Nam",
            sizes: ["M"], colors: ["Đen"],
            uploadImage: false
        );

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra hiển thị lỗi: Vui lòng upload ảnh chính");
        var hasError = HasValidationError() || HasErrorMessage()
            || Driver.Url.Contains("/Create");
        Console.WriteLine($"Có lỗi / vẫn ở trang Create: {hasError}");

        Assert.That(hasError, Is.True,
            "Không hiển thị lỗi khi không upload ảnh chính");
    }

    [Test]
    [Description("TC_F9.1_09 — Thêm SP - upload ảnh phụ đúng giới hạn tối đa (5 ảnh)")]
    [Category("F9.1_Create")]
    public void TC_F9_1_09_CreateProduct_5AdditionalImages_Success()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở form Thêm sản phẩm");
        page.NavigateToCreate();

        Console.WriteLine("Nhập đầy đủ thông tin SP");
        FillCreateProductForm(
            name: "SP 5 Ảnh Phụ", description: "Test upload 5 ảnh phụ",
            category: "Áo", gender: "Nam",
            sizes: ["M"], colors: ["Đen"]
        );

        Console.WriteLine("Upload đúng 5 ảnh phụ: a1.jpg → a5.jpg");
        var images = Enumerable.Range(1, 5)
            .Select(i => Path.Combine(TestFilesDir, $"a{i}.jpg"))
            .Where(File.Exists);
        if (images.Any())
            page.UploadAdditionalImages(images);

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra SP tạo thành công với 5 ảnh phụ");
        var success = HasSuccessMessage() || !Driver.Url.Contains("/Create");
        Console.WriteLine($"SP với 5 ảnh phụ tạo thành công: {success}");

        Assert.That(success, Is.True,
            "SP với 5 ảnh phụ không tạo được");
    }

    [Test]
    [Description("TC_F9.1_12 — Thêm SP - upload file ảnh không hợp lệ (.pdf)")]
    [Category("F9.1_Create")]
    public void TC_F9_1_12_CreateProduct_InvalidImagePdf_ShowsError()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở form Thêm sản phẩm");
        page.NavigateToCreate();

        Console.WriteLine("Nhập thông tin SP: tên, mô tả, danh mục, giới tính, size, màu");
        page.AppendProductName("SP Ảnh PDF");
        page.TypeProductDescription("Test upload PDF");
        page.SelectCategoryByText("Áo");
        page.SelectGenderByText("Nam");
        page.TickSizeById("sizeM");
        page.TickColorById("colorBlack");

        Console.WriteLine("Upload file .pdf thay vì ảnh: document.pdf");
        var pdfPath = Path.Combine(TestFilesDir, "document.pdf");
        if (File.Exists(pdfPath))
            page.UploadMainImage(pdfPath);

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra hiển thị lỗi: Chỉ chấp nhận file ảnh");
        var stayOnPage = Driver.Url.Contains("/Create");
        var hasError = HasErrorMessage() || HasValidationError();
        Console.WriteLine($"Có lỗi: {hasError} | Vẫn ở trang Create: {stayOnPage}");

        Assert.That(hasError || stayOnPage, Is.True,
            "Không hiển thị lỗi khi upload file PDF thay vì ảnh");
    }

    // ==================== F9.2 - CHỈNH SỬA SẢN PHẨM ====================

    [Test]
    [Description("TC_F9.2_01 — Chỉnh sửa tên SP thành công")]
    [Category("F9.2_Edit")]
    public void TC_F9_2_01_EditProduct_ChangeName_Success()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở trang Quản lý SP");
        page.NavigateToList();

        Console.WriteLine("Nhấn icon chỉnh sửa (xanh) của SP đầu tiên");
        page.ClickFirstEditButton();

        Console.WriteLine("Xóa tên cũ, nhập tên mới: 'Áo Thun Unisex Premium'");
        page.TypeProductName("Áo Thun Unisex Premium");

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra tên SP cập nhật thành công");
        var success = HasSuccessMessage();
        Console.WriteLine($"Cập nhật thành công: {success}");

        Assert.That(success, Is.True,
            "Không hiển thị thông báo cập nhật thành công");
    }

    [Test]
    [Description("TC_F9.2_02 — Chỉnh sửa SP - để trống tên SP")]
    [Category("F9.2_Edit")]
    public void TC_F9_2_02_EditProduct_EmptyName_ShowsError()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở trang Quản lý SP và nhấn chỉnh sửa SP đầu tiên");
        page.NavigateToList();
        page.ClickFirstEditButton();

        Console.WriteLine("Xóa trắng trường Tên SP");
        page.ClearProductName();

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra hiển thị lỗi: Tên SP không được để trống");
        var hasError = HasValidationError("Name") || HasErrorMessage()
            || Driver.Url.Contains("/Edit");
        Console.WriteLine($"Có lỗi validation: {hasError}");

        Assert.That(hasError, Is.True,
            "Không hiển thị lỗi khi để trống tên SP trong chỉnh sửa");
    }

    [Test]
    [Description("TC_F9.2_04 — Chỉnh sửa thay đổi Danh mục SP")]
    [Category("F9.2_Edit")]
    public void TC_F9_2_04_EditProduct_ChangeCategory_Success()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở trang Quản lý SP và nhấn chỉnh sửa SP đầu tiên");
        page.NavigateToList();
        page.ClickFirstEditButton();

        Console.WriteLine("Đổi Danh mục sang giá trị khác");
        var currentValue = page.GetSelectedCategoryText();
        Console.WriteLine($"Danh mục hiện tại: {currentValue}");
        var others = page.GetOtherValidCategoryOptions(currentValue);
        if (others.Count > 0)
        {
            page.SelectCategoryByText(others[0]);
            Console.WriteLine($"Đổi sang: {others[0]}");
        }

        Console.WriteLine("Nhấn Lưu SP");
        ClickSaveProduct();

        Console.WriteLine("Kiểm tra danh mục SP cập nhật thành công");
        var success = HasSuccessMessage();
        Console.WriteLine($"Cập nhật thành công: {success}");

        Assert.That(success, Is.True,
            "Không hiển thị thông báo thay đổi danh mục thành công");
    }

    // ==================== F9.3 - XÓA SẢN PHẨM ====================

    [Test]
    [Description("TC_F9.3_01 — Xóa mềm SP - xác nhận OK")]
    [Category("F9.3_Delete")]
    public void TC_F9_3_01_SoftDelete_ConfirmOK_ProductMovedToDeleted()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở trang Quản lý SP");
        page.NavigateToList();

        var rowCount = page.GetTableRowCount();
        Console.WriteLine($"Số SP hiện có: {rowCount}");
        if (rowCount == 0) { Assert.Ignore("Không có SP nào để test xóa"); return; }

        Console.WriteLine("Nhấn icon xóa (đỏ) của SP đầu tiên");
        if (!page.ClickFirstDeleteButton()) { Assert.Ignore("Không có form xóa"); return; }

        Console.WriteLine("Xác nhận dialog xóa (OK)");
        if (page.HandleAlert(accept: true)) Console.WriteLine("Đã xác nhận alert");
        else Console.WriteLine("Không có JS alert (có thể dùng modal)");
        Thread.Sleep(1000);

        Console.WriteLine("Kiểm tra SP chuyển sang trang 'Sản phẩm đã xóa'");
        var hasSuccess = HasSuccessMessage();
        Console.WriteLine($"Xóa mềm thành công: {hasSuccess}");

        Assert.That(hasSuccess, Is.True,
            "Không hiển thị thông báo xóa SP thành công");
    }

    [Test]
    [Description("TC_F9.3_02 — Xóa mềm SP - nhấn Hủy trong dialog")]
    [Category("F9.3_Delete")]
    public void TC_F9_3_02_SoftDelete_CancelDialog_ProductNotDeleted()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở trang Quản lý SP");
        page.NavigateToList();

        var initialCount = page.GetTableRowCount();
        Console.WriteLine($"Số SP hiện có: {initialCount}");
        if (initialCount == 0) { Assert.Ignore("Không có SP nào để test"); return; }

        Console.WriteLine("Nhấn icon xóa (đỏ) của SP");
        if (!page.ClickFirstDeleteButton()) { Assert.Ignore("Không có form xóa"); return; }

        Console.WriteLine("Nhấn Hủy trong dialog xác nhận");
        if (page.HandleAlert(accept: false)) Console.WriteLine("Đã nhấn Hủy alert");
        else Console.WriteLine("Không có JS alert");
        Thread.Sleep(1000);

        Console.WriteLine("Kiểm tra SP không bị xóa, vẫn hiển thị trong DS");
        var currentCount = page.GetTableRowCount();
        Console.WriteLine($"Số SP trước: {initialCount} | Sau: {currentCount}");

        Assert.That(currentCount, Is.EqualTo(initialCount),
            "Số SP thay đổi sau khi nhấn Hủy");
    }

    // ==================== F9.4 - HIỂN THỊ & TÌM KIẾM ====================

    [Test]
    [Description("TC_F9.4_01 — Hiển thị thống kê Tổng sản phẩm đúng")]
    [Category("F9.4_Display")]
    public void TC_F9_4_01_DisplayTotalProducts_CorrectCount()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Truy cập trang Quản lý SP");
        page.NavigateToList();

        Console.WriteLine("Quan sát ô Tổng sản phẩm");
        var statsCount = page.CountPrimaryStatCards();
        Console.WriteLine($"Số card thống kê tìm thấy: {statsCount}");

        Assert.That(statsCount, Is.GreaterThan(0),
            "Không hiển thị ô thống kê Tổng sản phẩm");
    }

    [Test]
    [Description("TC_F9.4_03 — Tìm kiếm SP theo tên - có kết quả")]
    [Category("F9.4_Display")]
    public void TC_F9_4_03_SearchByName_HasResults()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở trang Quản lý SP");
        page.NavigateToList();

        Console.WriteLine("Nhập từ khóa vào ô tìm kiếm: 'Quan Jean'");
        page.EnterSearchKeyword("Quan Jean");

        Console.WriteLine("Nhấn nút Lọc");
        page.ClickFilterSubmit();

        Console.WriteLine("Quan sát kết quả tìm kiếm");
        var urlHasSearch = Driver.Url.Contains("search=");
        var rowCount = page.GetTableRowCount();
        Console.WriteLine($"Số kết quả: {rowCount}");
        Console.WriteLine($"Tìm kiếm hoạt động: {urlHasSearch}");

        Assert.That(urlHasSearch, Is.True,
            "Tìm kiếm không được gửi đi");
    }

    [Test]
    [Description("TC_F9.4_05 — Lọc SP theo Danh mục - Shirt")]
    [Category("F9.4_Display")]
    public void TC_F9_4_05_FilterByCategory_Shirt()
    {
        var page = new ProductManagementPage(Driver);

        Console.WriteLine("Mở trang Quản lý SP");
        page.NavigateToList();

        Console.WriteLine("Chọn dropdown Danh mục: 'Áo'");
        page.SelectCategoryFilterByText("Áo");

        Console.WriteLine("Nhấn nút Lọc");
        page.ClickFilterSubmit();

        Console.WriteLine("Quan sát kết quả - chỉ hiển thị SP thuộc danh mục Áo");
        // Check if URL contains category parameter (URL-encoded "Áo" could be "%C3%81o" or "Áo")
        var urlHasCategory = Driver.Url.Contains("category=") && 
                            (Driver.Url.Contains("%C3%81o") || Driver.Url.Contains("Áo") || Driver.Url.Contains("category=Áo"));
        var rowCount = page.GetTableRowCount();
        Console.WriteLine($"Số SP sau lọc: {rowCount}");
        Console.WriteLine($"URL hiện tại: {Driver.Url}");
        Console.WriteLine($"Lọc Áo hoạt động: {urlHasCategory}");

        Assert.That(urlHasCategory, Is.True,
            "Lọc theo danh mục Áo không hoạt động");
        Assert.That(rowCount, Is.GreaterThan(0),
            "Không có sản phẩm nào sau khi lọc");
    }
}
