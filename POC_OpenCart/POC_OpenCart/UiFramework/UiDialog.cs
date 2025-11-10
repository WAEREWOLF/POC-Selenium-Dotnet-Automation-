using OpenQA.Selenium;
using System;
using System.Threading.Tasks;

namespace POC_OpenCart.UiFramework
{
    public class UiDialog : AbstractUiElement
    {
        #region .ctor
        public UiDialog(Browser browser, IWebElement element)
            : base(browser, element)
        { }
        #endregion


        #region Public methods
        public void SetFieldValueByPlaceholder(string text, string value)
        {
            var input = Element.FindElement(By.XPath($"//input[@placeholder='{text}']"));
           
            if (input == null)
            {
                throw new InvalidOperationException($"{this} does not have an input with the {text} placeholder.");
            }

            input.SendKeys(value);
        }

        public void SetFieldValueByDataPlaceholder(string text, string value)
        {
            var input = Element.FindElement(By.XPath($"//*[@data-placeholder='{text}']"));

            if (input == null)
            {
                throw new InvalidOperationException($"{this} does not have an input with the {text} placeholder.");
            }

            input.SendKeys(value);
        }

        public async Task ClickFieldBySpanText(string text, int wait = 1500)
        {
            var input = Element.FindElement(By.XPath($"//span[contains(text(), '{text}')]"));

            if (input == null)
            {
                throw new InvalidOperationException($"{this} does not have an input with the {text} span text.");
            }

            input.Click();
            await Task.Delay(wait);
        }

        public async Task ClickOption(string id, string optionValue, int wait = 1500)
        {
            IWebElement element = GetElementByDinamicId(id);
            var input = element.FindElement(By.XPath($"//span[contains(text(), '{optionValue}')]"));

            if (input == null)
            {
                throw new InvalidOperationException($"{this} does not have an input with the {optionValue} option name.");
            }

            input.Click();
            await Task.Delay(wait);
        }

        public async Task ClickOnButton(string btnName, int wait = 1000)
        {
            var element = FindElement(By.XPath($"//button[contains(text(), '{btnName}')]"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with name {btnName}.");
            }

            element.Click();

            await Task.Delay(wait);
        }

        //added
        public async Task ClickOnMyButtonByClass(string btnClass, int wait = 1000)
        {
            //var placeholder = "mat-chip-list-input-2";
            var element = FindElement(By.XPath($"//*[@id='mat-chip-list-input-2']"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with class name {btnClass}.");
            }

            element.Click();

            await Task.Delay(wait);
        }

        public async Task ClickOnButtonBySpan(string btnName,int wait = 1000)
        {
            var element = FindElement(By.XPath($"//span[contains(text(), '{btnName}')]"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain a button with span {btnName}.");
            }

            element.Click();

            await Task.Delay(wait);
        }
        #endregion

        #region Private
        private IWebElement GetElementByDinamicId(string id)
        {
            var element = Element.FindElement(By.XPath($"//*[starts-with(@id, '{id}')]"));
            if (element == null)
            {
                throw new InvalidOperationException($@"The page does not contain an element with id that starts with {id}.");
            }

            return element;
        }
        #endregion

        #region Selectors
        private const string ContainerAttribute = "class";

        public static By By_Container(string className)
        {
            return By.CssSelector($@"div[{ContainerAttribute}=""{className}""]");
        }

        public static By By_MatContainer(string className)
        {
            return By.CssSelector($@"mat-dialog-container[{ContainerAttribute}^=""{className}""]");
        }
        #endregion
    }
}
