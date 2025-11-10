using System.Threading.Tasks;
using POC_OpenCart.DataHelpers;
using POC_OpenCart.Framework;
using POC_OpenCart.UiFramework;
using NUnit.Framework;
using OpenQA.Selenium;
using POC_OpenCart.Constants;
using POC_OpenCart.Configuration;
using OpenQA.Selenium.Interactions;
using System.Xml.Linq;

namespace POC_OpenCart.Tests.Account
{
    [TestFixture]
    public class ProductsTests : ProductsTestHelper
    {
        #region UI Elements
        private const string PostCodeField = "postcode"; 
        private const string QuantityField = "quantity";
        private const string GetQuoteBtn = "//button[@id='button-quote']";
        private const string FlatRateRb = "input-shipping-method-flat-flat";
        private const string ApplyShipping = "button-shipping-method";
        private const string ConfirmBtn = "button-confirm";
        private const string SuccessOrderElement = "common-success";
        private const string AddToCartBtnn = "button-cart";

        private const int FlatRateValueNr = 5;        
        #endregion

        #region Test Data        
        private const string DesiredProduct1 = "iPhone";
        private const string DesiredProduct2 = "MacBook";
        private const string ExpectedDefaultQty = "1";
        private const string PostCodeValue = "453239";
        #endregion

        #region Methods 
        #endregion

        [OneTimeSetUp]
        public async Task Initialize()
        {
            await Browser.LogInOpenCart(ConfigManager.Email, ConfigManager.Password);

            //cleanup
            await DeleteAllProductsFromCart();
        }

        [Test, Order(0)]
        public async Task AddProductToCart()
        {
            //cleanup
            await DeleteAllProductsFromCart();

            await Browser.NavigateToPage(GeneralConstants.HomePage);
            ClickOnProduct(DesiredProduct1);

            //check default quantity is 1
            var productQuantity = Browser.GetInputTextByName(QuantityField);
            Assert.AreEqual(ExpectedDefaultQty, productQuantity);

            //retrieve price
            var productPrice = Browser.FindElement(Ui.By_Xpath("//*[@id='content']//span[@class='price-new']")).Text;

            //click Add to cart button
            await Browser.ClickOnButtonById(AddToCartBtnn);

            ClickShoppingCartBtn();

            var productName = Browser.FindElement(Ui.By_Xpath("//*[@id='shopping-cart']//table/tbody/tr[1]/td[2]/a")).Text;
            Assert.AreEqual(DesiredProduct1, productName);

            //check correct Qty
            var productQtyElement = Browser.FindElement(Ui.By_Xpath("//*[@id='shopping-cart']//table/tbody/tr[1]/td[4]"));
            var productQty = productQtyElement.FindElement(Ui.By_Xpath("//input[@name='quantity']")).GetAttribute("value");
            Assert.AreEqual(ExpectedDefaultQty, productQty);

            //check correct unit price
            var productCartUnitPrice = Browser.FindElement(Ui.By_Xpath("//*[@id='shopping-cart']//table/tbody/tr[1]/td[5]")).Text;
            Assert.AreEqual(productPrice, productCartUnitPrice);

            //check product total
            var productCartTotalPrice = Browser.FindElement(Ui.By_Xpath("//*[@id=\"checkout-total\"]/tr[4]/td[2]")).Text;
            Assert.AreEqual(productCartUnitPrice, productCartTotalPrice, "Total price on product " + DesiredProduct1 + " is wrong!");

           
        }

        [Test, Order(1)]
        public async Task RemoveAllProductsFromCart()
        {
            //Add products
            await AddProductToCartFromFeatures(DesiredProduct1);
            await AddProductToCartFromFeatures(DesiredProduct2);

            ClickShoppingCartBtn();
            await DeleteAllProductsFromCart();
            await Browser.Refresh();

            var elementsInCart = Browser.FindElement(Ui.By_Id("shopping-cart"));
            Assert.IsNull(elementsInCart, "Delete failed. There are still some elements in the cart!");
        }

        [Test, Order(2)]
        public async Task ApplyFlatShipingRate()
        {   
            await AddProductToCartFromFeatures(DesiredProduct1);
            ClickShoppingCartBtn();

            var initialTotal = Browser.FindElements(Ui.By_Xpath("//*[@id='checkout-total']/tr")).First().Text;

            //apply flat shipping
            await Browser.WaitAsync(1500);
            var estimateShipAndTaxBtn = Browser.FindElement(Ui.By_Xpath("//*[@id='accordion']/div/h2/button[@data-bs-target='#collapse-shipping']"));
            Assert.IsNotNull(estimateShipAndTaxBtn, "Estimate shipping and taxes button not found!");
            estimateShipAndTaxBtn.Click();

            var countryOption = Browser.FindElement(Ui.By_Xpath("//*[@id='input-country']/option[186]"));
            countryOption.Click();
            await Browser.WaitAsync(4000);
            
            var stateOption = Browser.FindElement(Ui.By_Xpath("//*[@id='input-zone']/option[19]"));
            stateOption.Click();

            Browser.SetFieldValueByName(PostCodeField, PostCodeValue);
            await Browser.WaitAsync(4000);

            var getQuoteBrn = Browser.FindElement(Ui.By_Xpath(GetQuoteBtn));
            Actions actions = new(WebDriverProvider.webDriver);
            actions.MoveToElement(getQuoteBrn);
            actions.Click().Perform();

            await Browser.ClickOnButtonById(FlatRateRb);    
            await Browser.ClickOnButtonById(ApplyShipping);

            var afterShippingAppliedTotal = Browser.FindElements(Ui.By_Xpath("//*[@id='checkout-total']/tr")).Last().Text;
            var initialTotalNr = GetNumber(initialTotal);
            var afterShippingAppliedTotalNr = GetNumber(afterShippingAppliedTotal);


            //Assert.AreEqual(Math.Round(FlatRateValueNr + initialTotalNr), Math.Round(afterShippingAppliedTotalNr), "Inital total and after applying flat rate is not correct!");
            Assert.That(Math.Round(FlatRateValueNr + initialTotalNr), Is.EqualTo(Math.Round(afterShippingAppliedTotalNr)).Within(2));
            
            //cleanup
            //await Browser.WaitAsync(4000);
            //await DeleteAllProductsFromCart();
        }

        [Test, Order(3)]
        [Description("The website could ban temporarly the access if run this test.")]
        //[Ignore("Can ban the access to the site.")]
       public async Task CheckOutProducts()
       {
            await AddProductToCartFromFeatures(DesiredProduct1);
            await AddProductToCartFromFeatures(DesiredProduct2);

            ClickShoppingCartBtn();

            var totalPriceList = new List<double>();
            var productsTotalPrice = Browser.FindElements(Ui.By_Xpath("//*[@id='shopping-cart']//table/tbody/tr/td[6]"));
            foreach (var productTotalStr in productsTotalPrice)
            {
                totalPriceList.Add(GetNumber(productTotalStr.Text));
            }

            var expectedTotalSum = totalPriceList.Sum();
            var actualTotalSumStr = Browser.FindElements(Ui.By_Xpath("//*[@id='checkout-total']/tr")).Last().Text;
            var actualTotalSum = GetNumber(actualTotalSumStr);

            Assert.AreEqual(Math.Round(expectedTotalSum), Math.Round(actualTotalSum), "The total sum of the products seems wrong!");

            var checkoutBtn = Browser.FindElement(Ui.By_Xpath("//*[@class='float-end']/a"));
            Assert.IsNotNull(checkoutBtn, "Checkout button not found!");
            checkoutBtn.Click();

            var flatRateOption = Browser.FindElement(Ui.By_Xpath("//*[@id='input-shipping-method']/optgroup/option"));
            Assert.IsNotNull(flatRateOption, "Flat rate option not found!");
            flatRateOption.Click();

            var paymentMethod = Browser.FindElement(Ui.By_Xpath("//*[@id='input-payment-method']/option[@value='cod']"));
            Assert.IsNotNull(paymentMethod, "Payment method value Cash on delivery not found!");
            paymentMethod.Click();

            Browser.Wait(4000);
            await Browser.ClickOnButtonById(ConfirmBtn);
            var successOrder = Browser.FindElement(Ui.By_Id(SuccessOrderElement));
            Assert.IsNotNull(successOrder, "The order is NOT successfull!");
        }

        [OneTimeTearDown]
        public async Task AfterSuite()
        {
            await Browser.LogOutOpenCart();
            Browser.Dispose();
        }        
    }
}
