using POC_OpenCart.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace POC_OpenCart.UiFramework
{
    public class Browser : ISearchContext, IDisposable
    {
        #region Private members
        private readonly IWebDriver _webDriver;
        #endregion


        #region .ctor
        public Browser(IWebDriver webDriver)
        {
            _webDriver = webDriver;
            _webDriver.Navigate().GoToUrl(ConfigManager.LoginUrl);
        }
        #endregion

        #region Login Methods
        public async Task LogInOpenCart(string username, string password)
        {
            var myAccountElement = FindElement(Ui.By_Xpath("//*[@id='content']/h2[contains(text(), 'My Account')]"));
            if (myAccountElement is null)
            {
                FindElement(By.XPath("//input[@id='input-email']")).SendKeys(username);
                FindElement(By.XPath("//input[@id='input-password']")).SendKeys(password);
                FindElement(By.XPath("//*[@id='form-login']//button")).Click();
            }

            await Task.Delay(2000);
        } 

        public async Task LogOutOpenCart()
        {
            await NavigateToPage("index.php?route=account/logout&language=en-gb");
        }
        #endregion

        #region Private Methods
        private WebDriverWait GetWait(TimeSpan? timeout = null)
        {
            return new WebDriverWait(_webDriver, TimeSpan.FromMilliseconds(2000));
        }
        #endregion

        #region ISearchContext
        public IWebElement FindElement(By by)
        {
            return FindElement(by, TimeSpan.FromMilliseconds(2000));
        }

        public IWebElement FindElement(By by, TimeSpan timeout)
        {
            IWebElement webElement = null;
            try
            {
                //webElement = GetWait(timeout).Until(ExpectedConditions.ElementIsVisible(by));
                webElement = GetWait(timeout).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

                return webElement;
            }
            catch (WebDriverTimeoutException)
            {
                //ignore
            }

            return webElement;
        }

        public IWebElement FindElement(IWebElement parentElement, By by, TimeSpan? timeout = null)
        {
            IWebElement webElement = null;
            try
            {
                webElement = GetWait(timeout).Until(driver =>
                {
                    IWebElement element = null;
                    try
                    {
                        element = parentElement.FindElement(by);
                    }
                    catch (NoSuchElementException)
                    {
                        //ignore
                    }
                    return (element?.Displayed).GetValueOrDefault() ? element : null;
                });
            }
            catch (WebDriverTimeoutException)
            {
                //ignore
            }

            return webElement;
        }

        public ReadOnlyCollection<IWebElement> FindElements(IWebElement parentElement, By by, TimeSpan? timeout = null, bool waitAllVisible = true)
        {
            ReadOnlyCollection<IWebElement> webElements = null;
            try
            {
                webElements = GetWait(timeout).Until(driver =>
                {
                    var elements = parentElement.FindElements(by);
                    return !waitAllVisible || (elements?.All(e => e.Displayed)).GetValueOrDefault() ? elements : null;
                });
            }
            catch (WebDriverTimeoutException)
            {
                //ignore
            }

            return webElements;
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return _webDriver.FindElements(by);
        }
        #endregion

        #region Navigate
        public async Task NavigateToScreen(string menuItem)
        {
            var element = FindElement(By.XPath($"//a/span[contains(text(), '{menuItem}')]"));

            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a screen with name {menuItem}.");
            }

            element.Click();

            await Task.Delay(1500);
        }

        public async Task NavigateToPage(string path, int wait = 1500)
        {
            _webDriver.Navigate().GoToUrl(ConfigManager.BaseUrl + path);

            await Task.Delay(wait);
        }

        public async Task GoTo(string url, int wait = 1500)
        {
            _webDriver.Navigate().GoToUrl(url);
            await Task.Delay(wait);
        }
        #endregion

        #region Public Methods
        public async Task ClickOnSpan(string btnName, int wait = 1000)
        {
            var element = FindElement(By.XPath($"//span[contains(text(), '{btnName}')]"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name {btnName}.");
            }

            element.Click();
            
            await Task.Delay(wait);
        }

        public async Task ClickOnButton(string btnName, int wait = 2000)
        {
            await Task.Delay(wait);
            var element = FindElement(By.XPath($"//button[contains(text(), '{btnName}')]"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name {btnName}.");
            }

            element.Click();

            await Task.Delay(wait);
        }
        
        public async Task ClickOnButtonById(string id, int wait = 1500)
        {
            var element = FindElement(By.Id(id));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with id {id}.");
            }
            
            element.Click();

            await Task.Delay(wait);
        }

        public async Task ClickOnButtonByXpath(string xPath, int wait = 1500)
        {
            var element = FindElement(By.XPath(xPath));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with xpath {xPath}.");
            }

            element.Click();

            await Task.Delay(wait);
        }

        public async Task ClickOnInput(string name, int wait = 1000)
        {
            var element = FindElement(By.CssSelector($"input[name='{name}']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name {name}.");
            }
            await WaitAsync(3000);
            element.Click();

            await Task.Delay(wait);
        }
              
        public string GetInputTextByName(string name)
        {
            var element = FindElement(By.CssSelector($"input[name='{name}']"));
            
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name {name}.");
            }

            return element.GetAttribute("value");
        }

        public string GetTableCellNameValue()
        {
            var element = FindElement(By.XPath($"//table/tbody/tr[1]/td[3]"));

            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name.");
            }

            return element.Text;
        }
       
        public async Task DoubleClickOnRow(int row, int wait = 1000)
        {
            var element = FindElement(By.XPath($"//table/tbody/tr[{row}]"));

            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name.");
            }
            var action = new Actions(_webDriver);
            action.DoubleClick(element).Build().Perform();
            
            await Task.Delay(wait); 
        }

        public async Task DoubleClickButton(string btnName, int wait = 1000)
        {
            var element = FindElement(By.XPath($"//span[contains(text(), '{btnName}')]"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name {btnName}.");
            }

            var action = new Actions(_webDriver);
            action.DoubleClick(element);
            
            await Task.Delay(wait);
        }
       
        public async Task ClickButtonByName(string btnName, int wait = 1000)
        {
            var element = FindElement(By.XPath($"//button[contains(text(), '{btnName}')]"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name {btnName}.");
            }

            element.Click();
            await Task.Delay(wait);
        }
        
        public async Task DragAndDropElement(string sourceName, string targetName, int wait = 1000)
        {
            var sourceElement = FindElement(By.XPath($"(//div[contains(text(), '{sourceName}')])[2]"));
            var targetElement = FindElement(By.XPath($"(//div[contains(text(), '{targetName}')])[2]"));

            if (sourceElement == null)
            {
                throw new InvalidOperationException($@"The page does not contain an item  with name {sourceName}.");
            }
            if (targetElement == null)
            {
                throw new InvalidOperationException($@"The page does not contain an item with name {sourceName}.");
            }

            var action = new Actions(_webDriver);
            action.DragAndDrop(sourceElement, targetElement).Build().Perform();

            await Task.Delay(wait);
        }

        public async Task ClickOnButtonByClass(string btnClass, int wait = 1000)
        {
            var element = FindElement(By.CssSelector($"button[class^='{btnClass}']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with class name {btnClass}.");
            }

            element.Click();

            await Task.Delay(wait);
        }
        
        public async Task ClickOnMyDivByClass(string btnClass, int wait = 1000)
        {            
            var element = FindElement(By.CssSelector($"div[class^='{btnClass}']"));

            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with class name {btnClass}.");
            }

            element.Click();

            await Task.Delay(wait);
        }
        
        public async Task ClickOnDivByName(string name, int wait = 1000)
        {
            var element = FindElement(By.XPath($"//div[contains(text(), '{name}')]"));

            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with class name {name}.");
            }

            element.Click();

            await Task.Delay(wait);
        }

        public async Task ClickOnCheckboxByText(string text)
        {
            var element = FindElement(By.XPath($"//span[contains(text(), '{text}')]"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a checkbox with label {text}.");
            }

            element.Click();

            await Task.Delay(1500);
        }

        public UiGrid GetGrid(string gridClassName)
        {
            var element = FindElements(UiGrid.By_Container(gridClassName));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a {gridClassName} grid.");
            }

            return new UiGrid(this, element[1]);
        }

        public UiGrid GetGridHeader(string className)
        {
            var element = FindElements(UiGrid.By_HeaderContainer(className));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a {className} grid.");
            }

            return new UiGrid(this, element[1]);
        }

        public UiGrid GetMatGrid(string gridClassName)
        {
            var element = FindElement(UiGrid.By_MatContainer(gridClassName));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a {gridClassName} grid.");
            }

            return new UiGrid(this, element);
        }

        public string GetRecordsCount()
        {
            var element = FindElement(By.CssSelector(@".p-paginator-right-content"));

            if (element == null)
            {
                return string.Empty;
            }

            return element.Text.Split("of").Last();
        }

        public UiDialog GetDialog(string className)
        {
            var element = FindElement(UiDialog.By_Container(className));

            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a dialog with {className}.");
            }

            return new UiDialog(this, element);
        }

        public UiDialog GetMatDialog(string id)
        {
            var element = FindElement(UiDialog.By_MatContainer(id));

            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a dialog with {id}.");
            }

            return new UiDialog(this, element);
        }
        public UiDialog GetDialogByID(string id)
        {
            var element = FindElement(By.Id(id));

            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a dialog with {id}.");
            }

            return new UiDialog(this, element);
        }

        public async Task ClickOnMatButtonByClass(string btnClass)
        {
            var element = FindElement(By.CssSelector($"mat-button-toggle[class^='{btnClass}']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with class name {btnClass}.");
            }

            element.Click();

            await Task.Delay(1500);
        }

        public async Task ClickOnMatOptionByClass(string btnClass)
        {
            var element = FindElements(By.CssSelector($"span[class='{btnClass}']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with class name {btnClass}.");
            }

            element[1].Click();

            await Task.Delay(500);
        }

        public void SetFieldValueByDataPlaceholder(string placeholder, string value)
        {
            var element = FindElement(By.XPath($"//*[@data-placeholder='{placeholder}']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain an element with data placeholder {placeholder}.");
            }

            element.SendKeys(value);
        }
       
        public void UpdateFieldValueByDataPlaceholder(string placeholder, string value)
        {
            var element = FindElement(By.XPath($"//*[@data-placeholder='{placeholder}']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain an element with data placeholder {placeholder}.");
            }

            element.Clear();
            element.SendKeys(value);
            Thread.Sleep(500);
        }

        public void SetFieldValueByClass(string className, string value)
        {
            var element = FindElement(By.XPath($"//*[@class='{className}']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain an element with class name {className}.");
            }

            element.SendKeys(value);
        }
       
        public void SetFieldValueByName(string name, string value)
        {
            var element = FindElement(By.XPath($"//input[@name='{name}']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain an element with class name {name}.");
            }
            element.Clear();
            element.SendKeys(value);
        }
       
        public void SelectDropdownOption(string name)
        {
            
            var element = FindElements(By.TagName("li")).FirstOrDefault(a => a.Text == name);
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a dropdown element with option name {name}.");
            }

            element.Click();
        }

        public WebDriverWait Wait(double milliseconds)
        {
            return new WebDriverWait(_webDriver, TimeSpan.FromMilliseconds(milliseconds));
        }

        public async Task WaitAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }

        public async Task Refresh()
        {
           _webDriver.Navigate().Refresh();
            await WaitAsync(1500);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _webDriver.Dispose();
            _webDriver.Quit();
        }
        #endregion
    }
}
