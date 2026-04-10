using OpenQA.Selenium;
using System.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace SeleniumProject.Pages
#pragma warning restore IDE0130
{
    public class ProductDetailsPage(IWebDriver driver) : BasePage(driver)
    {
        private readonly By btnAddToCart = By.CssSelector("button[type='submit'].btn-dark");
        private readonly By btnBuyNow = By.CssSelector("button[type='submit'].btn-danger");
        private readonly By inputQuantity = By.Id("quantityInput");

        public void SelectColor(string color)
        {
            var locator = By.XPath($"//button[contains(@class,'color-option') and normalize-space()='{color}']");
            var btn = WaitForElementVisible(locator);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
            System.Threading.Thread.Sleep(500);
            btn.Click();
        }

        public void SelectColor(int index)
        {
            var locator = By.XPath($"(//button[contains(@class,'color-option')])[{index + 1}]");
            var btn = WaitForElementVisible(locator);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
            System.Threading.Thread.Sleep(500);
            btn.Click();
        }

        public void SelectSize(string size)
        {
            var locator = By.XPath($"//button[contains(@class,'size-option') and normalize-space()='{size}']");
            var btn = WaitForElementVisible(locator);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
            System.Threading.Thread.Sleep(500);
            btn.Click();
        }

        public void SelectSize(int index)
        {
            var locator = By.XPath($"(//button[contains(@class,'size-option')])[{index + 1}]");
            var btn = WaitForElementVisible(locator);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
            System.Threading.Thread.Sleep(500);
            btn.Click();
        }

        public void SetQuantity(string qty)
        {
            var input = WaitForElementVisible(inputQuantity);
            // Xóa giá trị cũ bằng cách select all rồi ghi đè
            input.Click();
            input.SendKeys(Keys.Control + "a");
            input.SendKeys(qty);
        }

        public int GetMaxStock()
        {
            try {
                var input = driver.FindElement(inputQuantity);
                string? max = input.GetAttribute("max");
                return int.TryParse(max, out int m) ? m : 0;
            } catch { return 0; }
        }

        public bool IsAddToCartBlocked()
        {
            try {
                var alert = driver.SwitchTo().Alert();
                alert.Accept();
                return true;
            } catch { }
            return driver.FindElements(By.CssSelector(".alert-danger, .text-danger, .swal2-html-container, .toast-error"))
                         .Any(e => e.Displayed);
        }

        public void ClickAddToCart()
        {
            Console.WriteLine("Đang nhấn 'THÊM VÀO GIỎ HÀNG'...");
            var btn = WaitForElementVisible(btnAddToCart);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
            System.Threading.Thread.Sleep(500); 
            try {
                btn.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
            }
        }

        public void ClickBuyNow()
        {
            Console.WriteLine("Đang nhấn 'MUA NGAY'...");
            var btn = WaitForElementVisible(btnBuyNow);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", btn);
            System.Threading.Thread.Sleep(1000);
            try {
                btn.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
            }
        }

        // Review verification methods
        public bool HasReviewWithContent(string content)
        {
            try {
                // Check in review cards - based on HTML structure provided
                // Reviews are in cards with class "card mb-3" containing the comment text
                var reviewElements = driver.FindElements(By.XPath($"//div[contains(@class,'card')]//p[contains(.,'{content}')]"));
                foreach (var element in reviewElements)
                {
                    if (element.Displayed) return true;
                }
                
                // Also check in any element with text-muted class (where review content is displayed)
                var mutedElements = driver.FindElements(By.XPath($"//p[contains(@class,'text-muted') and contains(.,'{content}')]"));
                foreach (var element in mutedElements)
                {
                    if (element.Displayed) return true;
                }
                
                return false;
            } catch {
                return false;
            }
        }

        public string GetProductRating()
        {
            try {
                var ratingElement = driver.FindElement(By.XPath("//span[contains(@class,'rating') or contains(@class,'star')]"));
                return ratingElement.Text;
            } catch {
                return "";
            }
        }

        public void NavigateTo(string productId)
        {
            driver.Navigate().GoToUrl($"{driver.Url.Split("/Products")[0]}/Products/Details/{productId}");
            System.Threading.Thread.Sleep(1000);
        }

        // Review form methods (on product details page)
        public void SelectRating(int stars)
        {
            // Click vào label để chọn rating (1-5 stars)
            // Label có attribute for="star{n}", không phải id="star{n}"
            var starLabel = WaitForElementClickable(By.CssSelector($"label[for='star{stars}']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", starLabel);
            System.Threading.Thread.Sleep(300);
            
            // Click vào label thay vì radio button
            try {
                starLabel.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", starLabel);
            }
            System.Threading.Thread.Sleep(500);
        }

        public void EnterReviewContent(string content)
        {
            var textarea = WaitForElementVisible(By.Name("comment"));
            textarea.Clear();
            textarea.SendKeys(content);
            System.Threading.Thread.Sleep(500);
        }

        public void SubmitReview()
        {
            var submitBtn = WaitForElementClickable(By.XPath("//form[contains(@action,'Reviews/Add')]//button[@type='submit']"));
            
            // Scroll to button
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", submitBtn);
            System.Threading.Thread.Sleep(300);
            
            try {
                submitBtn.Click();
            } catch {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitBtn);
            }
            System.Threading.Thread.Sleep(2000);
        }

        public bool IsReviewFormDisplayed()
        {
            try {
                // Check if form exists
                var form = driver.FindElements(By.XPath("//form[contains(@action,'Reviews/Add')]"));
                if (form.Count == 0) return false;
                
                // Check if rating inputs exist
                var ratingInputs = driver.FindElements(By.Name("rating"));
                if (ratingInputs.Count == 0) return false;
                
                // Check if comment textarea exists
                var commentTextarea = driver.FindElements(By.Name("comment"));
                foreach (var element in commentTextarea)
                {
                    if (element.Displayed) return true;
                }
                return false;
            } catch {
                return false;
            }
        }

        public bool HasSuccessMessage()
        {
            try {
                var successAlert = driver.FindElements(By.CssSelector(".alert-success"));
                foreach (var element in successAlert)
                {
                    if (element.Displayed) return true;
                }
                return false;
            } catch {
                return false;
            }
        }

        // Review list methods
        public void ScrollToReviewsSection()
        {
            try {
                // Scroll to reviews tab or section
                var reviewsTab = driver.FindElement(By.CssSelector("button[data-bs-target='#reviews'], a[href='#reviews'], #reviews"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", reviewsTab);
                System.Threading.Thread.Sleep(500);
            } catch {
                // If no tab, just scroll down
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 500);");
                System.Threading.Thread.Sleep(500);
            }
        }

        public void ClickReviewsTab()
        {
            try {
                var reviewsTab = WaitForElementClickable(By.CssSelector("button[data-bs-target='#reviews']"));
                reviewsTab.Click();
                System.Threading.Thread.Sleep(1000);
            } catch {
                // Tab might already be active or doesn't exist
            }
        }

        public int GetReviewCount()
        {
            try {
                // Count review cards
                var reviewCards = driver.FindElements(By.XPath("//div[@id='reviews']//div[contains(@class,'card mb-3')]"));
                return reviewCards.Count;
            } catch {
                return 0;
            }
        }

        public bool HasReviewList()
        {
            try {
                var reviewCards = driver.FindElements(By.XPath("//div[@id='reviews']//div[contains(@class,'card')]"));
                return reviewCards.Count > 0;
            } catch {
                return false;
            }
        }

        public bool ReviewContainsUserName(int reviewIndex = 0)
        {
            try {
                var userNameElements = driver.FindElements(By.XPath("//div[@id='reviews']//div[contains(@class,'card')]//h6"));
                return reviewIndex < userNameElements.Count && !string.IsNullOrWhiteSpace(userNameElements[reviewIndex].Text);
            } catch {
                return false;
            }
        }

        public bool ReviewContainsStars(int reviewIndex = 0)
        {
            try {
                // Get all review cards
                var reviewCards = driver.FindElements(By.XPath("//div[@id='reviews']//div[contains(@class,'card')]"));
                if (reviewIndex >= reviewCards.Count) return false;
                
                // Check if the specific review card has star elements
                var starElements = reviewCards[reviewIndex].FindElements(By.XPath(".//i[contains(@class,'fa-star')]"));
                return starElements.Count > 0;
            } catch {
                return false;
            }
        }

        public bool ReviewContainsContent(int reviewIndex = 0)
        {
            try {
                var contentElements = driver.FindElements(By.XPath("//div[@id='reviews']//div[contains(@class,'card')]//p[contains(@class,'text-muted')]"));
                return reviewIndex < contentElements.Count && !string.IsNullOrWhiteSpace(contentElements[reviewIndex].Text);
            } catch {
                return false;
            }
        }

        public bool ReviewContainsDate(int reviewIndex = 0)
        {
            try {
                var dateElements = driver.FindElements(By.XPath("//div[@id='reviews']//div[contains(@class,'card')]//small[contains(@class,'text-muted')]"));
                return reviewIndex < dateElements.Count && !string.IsNullOrWhiteSpace(dateElements[reviewIndex].Text);
            } catch {
                return false;
            }
        }
    }
}
