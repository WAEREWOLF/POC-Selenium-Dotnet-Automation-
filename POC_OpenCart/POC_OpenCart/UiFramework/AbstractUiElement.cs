using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace POC_OpenCart.UiFramework
{
    public abstract class AbstractUiElement : ISearchContext
    {
        #region Private members
        protected Browser Browser { get; }

        protected IWebElement Element { get; }

        #endregion


        #region .ctor
        protected AbstractUiElement(Browser browser, IWebElement element)
        {
            Browser = browser;
            Element = element;
        }
        #endregion


        #region ISearchContext
        public IWebElement FindElement(By by)
        {
            return Browser.FindElement(Element, by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By @by)
        {
            return FindElements(by, true);
        }

        public IWebElement FindElement(By by, TimeSpan timeout)
        {
            return Browser.FindElement(Element, by, timeout);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by, bool waitAllVisible)
        {
            return Browser.FindElements(Element, by, null, waitAllVisible);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by, TimeSpan timeout, bool waitAllVisible = true)
        {
            return Browser.FindElements(Element, by, timeout);
        }
        #endregion
    }
}