using OpenQA.Selenium;

namespace POC_OpenCart.UiFramework
{
    public class UiGrid : AbstractUiElement
    {
        #region .ctor
        public UiGrid(Browser browser, IWebElement element)
            : base(browser, element)
        { }
        #endregion


        #region Public methods
        public IWebElement GetLastColumn()
        {
            return Element.FindElement(By.CssSelector(@"table")).FindElement(By.XPath("..//th[last()]"));
        }

        public IEnumerable<IWebElement> GetRows()
        {
            var elements = Element.FindElements(By.TagName("tr"));
            if (elements == null)
            {
                throw new InvalidOperationException($"{this} does not have rows.");
            }

            return elements;
        }

        public IEnumerable<IWebElement> GetMatRows()
        {
            var elements = Element.FindElements(By.TagName("mat-row"));
            if (elements == null)
            {
                throw new InvalidOperationException($"{this} does not have rows.");
            }

            return elements;
        }
        
        public IEnumerable<IWebElement> GetMatTableHeader()
        {
            var elements = Element.FindElements(By.TagName("mat-header-row"));
            if (elements == null)
            {
                throw new InvalidOperationException($"{this} does not have header row.");
            }

            return elements;
        }

        public IEnumerable<IWebElement> GetMatCells()
        {
            var elements = Element.FindElements(By.TagName("mat-cell"));
            if (elements == null)
            {
                throw new InvalidOperationException($"{this} does not have rows.");
            }

            return elements;
        }

        public string GetCellValue(int rowNr, int columnNr)
        {
            return GetCellElement(rowNr, columnNr).Text;
        }

        public string GetMatCellValue(int rowNr, int columnNr)
        {
            return GetMatCellElement(rowNr, columnNr).Text;
        }
        
        public string GetMatCellHedearValue(int columnNr)
        {
            return GetMatCellHeaderElement(columnNr).Text;
        }

        public async Task ClickCell(int rowNr, int columnNr)
        {
            var element = GetCellElement(rowNr, columnNr);
            element.Click();

            await Task.Delay(1000);
        }

        public async Task ClickCellCheckbox(string id)
        {             
            var element = FindElement(By.Id($"mat-checkbox-{id}"));
            element.Click();

            await Task.Delay(1000);
        }               

        public async Task PerformSort(string columnName)
        {
            var element = FindElement(By_Header(columnName));

            if (element == null)
            {
              throw new InvalidOperationException($"{this} does not have a {columnName} column header.");
            }

            element.Click();

            await Task.Delay(500);
        }

        public async Task ClickFilterColumn(string columnName)
        {
            var element = FindElement(By.CssSelector($@"table thead th[ng-reflect-field=""{columnName}""] p-columnfilter div button"));

            if (element == null)
            {
                throw new InvalidOperationException($"{this} does not have a {columnName} column header.");
            }

            element.Click();

            await Task.Delay(500);
        }
        #endregion


        #region Private methods
        private IWebElement GetCellElement(int rowNr, int columnNr)
        {
            var columnDivs = Element.FindElements(By.XPath($"//tr[{rowNr}]/td/div"));
            if (columnDivs == null)
            {
                throw new InvalidOperationException($"A cell value with the column number {columnNr} doesn't exists.");
            }
            return columnDivs[columnNr];
        }
        //ADDED
        private IWebElement GetCellCheckboxElement(int rowNr, int columnNr)
        {
            var columnDivs = Element.FindElements(By.XPath($"//*[@id='mat-checkbox-2-input']")).First();
            if (columnDivs == null)
            {
                throw new InvalidOperationException($"A cell value with the column number {columnNr} doesn't exists.");
            }
            return columnDivs;
        }

        private IWebElement GetMatCellElement(int rowNr, int columnNr)
        {
            var columnDivs = Element.FindElements(By.XPath($"//mat-row[{rowNr}]/mat-cell/span/div"));
            if (columnDivs == null)
            {
                throw new InvalidOperationException($"A cell value with the column number {columnNr} doesn't exists.");
            }
            return columnDivs[columnNr];
        }
        //ADDED
        private IWebElement GetMatCellHeaderElement(int columnNr)
        {
            var columnDivs = Element.FindElements(By.XPath($"//mat-header-row/mat-header-cell/div/div/div/div/div[contains(@class, 'mat-sort-header-content')]"));
            if (columnDivs == null)
            {
                throw new InvalidOperationException($"A cell value with the column number {columnNr} doesn't exists.");
            }
            return columnDivs[columnNr];
        }
        #endregion


        #region Selectors
        private const string ContainerAttribute = "class";
        private const string HeaderAttribute = "ng-reflect-field";

        private static By By_Header(string columnName)
        {
            return By.CssSelector($@"table thead th[{HeaderAttribute}=""{columnName}""]");
        }

        public static By By_Container(string gridClassName)
        {
            return By.CssSelector($@"table tbody[{ContainerAttribute}=""{gridClassName}""]");
        }

        public static By By_HeaderContainer(string className)
        {
            return By.CssSelector($@"table thead tr[{ContainerAttribute}=""{className}""]");
        }

        public static By By_MatContainer(string gridClassName)
        {
            return By.CssSelector($@"mat-table[{ContainerAttribute}=""{gridClassName}""]");
        }
        #endregion
    }
}
