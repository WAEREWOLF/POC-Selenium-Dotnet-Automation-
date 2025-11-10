using Microsoft.VisualStudio.TestTools.UnitTesting;
using POC_OpenCart.Constants;
using POC_OpenCart.Framework;
using POC_OpenCart.UiFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace POC_OpenCart.Tests
{
    public class ProductsTestHelper : BrowserTestFixture
    {
        public double GetNumber(string input)
        {
            var tempStr = new string(input.Where(c => char.IsDigit(c) || c.Equals('.')).ToArray());
            return Convert.ToDouble(tempStr);
        }

        public void ClickShoppingCartBtn()
        {
            var shoppingCartBtn = Browser.FindElement(Ui.By_Xpath("//a[@title='Shopping Cart']"));
            Assert.IsNotNull(shoppingCartBtn, "Shopping cart button not found!");
            shoppingCartBtn.Click();
        }

        public void ClickOnProduct(string productTitle)
        {
            var product = Browser.FindElement(Ui.By_Xpath($"//*[@id='content']//a/img[@title='{productTitle}']"));
            Assert.IsNotNull(product, "The product with title " + productTitle + " was not found!");
            product.Click();
        }

        public async Task DeleteAllProductsFromCart()
        {
            ClickShoppingCartBtn();
            var productsRow = Browser.FindElements(Ui.By_Xpath("//*[@id=\"output-cart\"]/table/tbody/tr/td[3]/form/div/a"));
            
            for (int i = 1; i <= productsRow.Count; i++)
            {
                await Browser.WaitAsync(1500);
                var productDelete = Browser.FindElement(Ui.By_Xpath($"//*[@id=\"output-cart\"]/table/tbody/tr/td[3]/form/div/a"));
                productDelete.Click();
            }
            await Browser.WaitAsync(1500);
        }

        public async Task AddProductToCartFromFeatures(string desiredProduct)
        {
            await Browser.NavigateToPage(GeneralConstants.HomePage);
            ClickOnProduct(desiredProduct);
            await Browser.ClickOnButtonById("button-cart");
        }
    }
}
