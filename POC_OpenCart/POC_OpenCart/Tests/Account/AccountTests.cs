using System.Threading.Tasks;
using POC_OpenCart.DataHelpers;
using POC_OpenCart.Framework;
using POC_OpenCart.UiFramework;
using NUnit.Framework;
using OpenQA.Selenium;
using POC_OpenCart.Constants;
using POC_OpenCart.Configuration;

namespace POC_OpenCart.Tests.Account
{
    [TestFixture]
    public class AccountTests : BrowserTestFixture
    {
        #region UI Elements       
        private const string FirstNameField = "firstname";
        private const string LastNameField = "lastname";
        private const string CompanyField = "company";
        private const string Address1Field = "address_1";
        private const string CityField = "city";
        private const string PostCodeField = "postcode";
        private const string ContinueBtnName = "Continue";
        private const string WebsiteField = "website";
        private const string TaxField = "tax";
        private const string PaypalField = "paypal";

        private const int EditAccountBtnIndex = 2;
        #endregion

        #region Test Data
        private const string FirstName = "Demo2";
        private const string LastName = "Customer2";
        private const string Company = "Netrom";
        private const string Address1 = "Caracal 178";
        private const string City = "Craiova";
        private const string PostCode = "200542";
        private const int RegionDropDownValue = 175;
        private  string NewFirstName = "Demo " + DateTime.Now.Minute.ToString();
        private  string AffiliateCompany = "Netrom " + DateTime.Now.Minute.ToString();
        private const string TaxId = "1234";
        private const string PaypalEmail = "netromAffiliate@gmail.com";
        private const string MySite = "mysite.com";
        private const string DesiredProduct = "MacBook";
        #endregion

        #region Methods
        private void ClickAccountMenuItem(int index)
        {
            var buttonsContainer = Browser.FindElement(Ui.By_Id("column-right"));
            var menuItemBtn = buttonsContainer.FindElement(Ui.By_Xpath($".//a[{index}]"));
            Assert.IsNotNull(menuItemBtn, "Account menu item button index " + index + " is not found!");
            Browser.Wait(1500);
            menuItemBtn.Click();
        }

        private void ClickAffiliateInfo()
        {
            var editAffInfoLink = Browser.FindElement(Ui.By_Xpath("//*[@id='content']//a[contains(text(), 'Register for an affiliate account')]"));
            if (editAffInfoLink == null)
            {
                editAffInfoLink = Browser.FindElement(Ui.By_Xpath("//*[@id='content']//a[contains(text(), 'Edit your affiliate information')]"));
            }
            Assert.IsNotNull(editAffInfoLink);
            editAffInfoLink.Click();
        }

        private void ClickOnProduct(string productTitle)
        {
            var product = Browser.FindElement(Ui.By_Xpath($"//*[@id='content']//a/img[@title='{productTitle}']"));
            Assert.IsNotNull(product, "The product with title " + productTitle + " was not found!");
            product.Click();
        }

        public async Task DeleteAllAddresses()
        {
            ClickAddressBook();
            var addressRows = Browser.FindElements(Ui.By_Xpath("//*[@id='address']//table/tbody/tr//a[@class='btn btn-danger']"));

            if (addressRows.Count > 1)
            {
                for (int i = 1; i < addressRows.Count; i++)
                {
                    await Browser.WaitAsync(1500);
                    var addressDelete = Browser.FindElements(Ui.By_Xpath($"//*[@id='address']//table/tbody/tr//a[@class='btn btn-danger']")).Last();
                    addressDelete.Click();

                    await Browser.WaitAsync(500);
                    var successAlert = Browser.FindElement(Ui.By_Xpath("//*[@id='content']/div[@class='alert alert-success alert-dismissible']"));
                    Assert.IsNotNull(successAlert);

                    await Browser.WaitAsync(1500);
                    ClickAddressBook();
                }
            }
        }

        public void ClickAddressBook()
        {
            var buttonsContainer = Browser.FindElement(Ui.By_Id("column-right"));
            var addressBookBtn = buttonsContainer.FindElement(Ui.By_Xpath("//a[text()='Address Book']"));
            Assert.IsNotNull(addressBookBtn);
            addressBookBtn.Click();
        }
        #endregion

        [OneTimeSetUp]
        public async Task Initialize()
        {
            await Browser.LogInOpenCart(ConfigManager.Email, ConfigManager.Password);

            //cleanup
            //await DeleteAllAddresses();
        }

        [Test, Order(0)]
        public async Task AddAddressBookEntry()
        {
            ClickAddressBook();

            var newAddressBtn = Browser.FindElement(Ui.By_Xpath("//*[@class='btn btn-primary']"));
            Assert.IsNotNull(newAddressBtn);
            newAddressBtn.Click();

            Browser.SetFieldValueByName(FirstNameField, FirstName);
            Browser.SetFieldValueByName(LastNameField, LastName);
            Browser.SetFieldValueByName(CompanyField, Company);
            Browser.SetFieldValueByName(Address1Field, Address1);
            Browser.SetFieldValueByName(CityField, City);
            Browser.SetFieldValueByName(PostCodeField, PostCode);

            await Browser.WaitAsync(2000);
            var dropDownValue = Browser.FindElement(Ui.By_Xpath($"//option[@value = {RegionDropDownValue}]"));
            Assert.IsNotNull(dropDownValue, "Dropdown value not found!");
            dropDownValue.Click();

            Browser.Wait(1500);
            await Browser.ClickOnButton(ContinueBtnName);

            ClickAddressBook();
            var entryInserted = Browser.FindElement(Ui.By_Xpath($"//*[@id='address']//table//td[contains(text(), '{FirstName + " " + LastName}')]"));
            Assert.IsNotNull(entryInserted, "Created address book with customer name: " + FirstName + LastName + " has failed!");
        }        

        [Test, Order(1)]
        public async Task EditAccountName()
        {
            //await Browser.GoTo(GeneralConstants.AccountPage);

            ClickAccountMenuItem(EditAccountBtnIndex);

            var firstNameTextBox = Browser.FindElement(Ui.By_Id("input-firstname"));
            Assert.IsNotNull(firstNameTextBox);
            firstNameTextBox.Clear();
            firstNameTextBox.SendKeys(NewFirstName);

            var continueBtn = Browser.FindElement(Ui.By_Xpath("//*[@id='form-customer']//button"));
            Assert.IsNotNull(continueBtn);
            continueBtn.Click();

            await Browser.WaitAsync(2000);
            ClickAccountMenuItem(EditAccountBtnIndex);

            var firstNameTextBox2 = Browser.FindElement(Ui.By_Id("input-firstname"));
            Assert.IsNotNull(firstNameTextBox2);
            Assert.AreEqual(NewFirstName, firstNameTextBox2.GetAttribute("value"));
            
            var backBtn = Browser.FindElement(Ui.By_Xpath("//*[@id='form-customer']//a"));
            Assert.IsNotNull(backBtn);
            backBtn.Click();
        }

        [Test, Order(2)]
        public async Task EditAffiliateInformation()
        {
           //await Browser.GoTo(GeneralConstants.AccountPage);

            ClickAffiliateInfo();

            Browser.SetFieldValueByName(CompanyField, AffiliateCompany);
            Browser.SetFieldValueByName(WebsiteField, MySite);
            Browser.SetFieldValueByName(TaxField, TaxId);

            var paypalRb = Browser.FindElement(Ui.By_Id("input-payment-paypal"));
            Assert.IsNotNull(paypalRb);
            paypalRb.Click();

            Browser.SetFieldValueByName(PaypalField, PaypalEmail);
            
            var acceptPolicyBtn = Browser.FindElement(Ui.By_Xpath("//*[@id=\"input-agree\"]"));
            if (acceptPolicyBtn != null)
            {
                acceptPolicyBtn.Click();
            }

            var continueBtn = Browser.FindElement(Ui.By_Xpath("//*[@id='form-affiliate']//button"));
            Assert.IsNotNull(continueBtn);
            continueBtn.Click();

            await Browser.WaitAsync(1500);

            ClickAffiliateInfo();
            var companyFieldValue = Browser.GetInputTextByName(CompanyField);
            Assert.AreEqual(AffiliateCompany, companyFieldValue);

            var paypalRb2 = Browser.FindElement(Ui.By_Id("input-payment-paypal"));
            Assert.IsTrue(paypalRb2.Selected == true);
        }

        [Test, Order(3)]
        public async Task AddProductToWishlist()
        {
            await Browser.NavigateToPage(GeneralConstants.HomePage);
            ClickOnProduct(DesiredProduct);

            await Browser.WaitAsync(1500);
            var addToWishlistBtn = Browser.FindElement(Ui.By_Xpath("//*[@id=\"content\"]//button[@title='Add to Wish List']"));
            Assert.IsNotNull(addToWishlistBtn);
            addToWishlistBtn.Click();

            var userWishListBtn = Browser.FindElement(Ui.By_Id("wishlist-total"));
            Assert.IsNotNull(userWishListBtn);
            userWishListBtn.Click();

            var productInWishlist = Browser.FindElement(Ui.By_Xpath($"//*[@id='wishlist']//td[2]/a[contains(text(),'{DesiredProduct}')]"));
            Assert.IsNotNull(productInWishlist, "The product " + DesiredProduct + " does not exist in the wish list!");
        }

        [Test, Order(4)]
        public async Task RemoveProductFromWishlist()
        {            
            await AddProductToWishlist();
            
            var userWishListBtn = Browser.FindElement(Ui.By_Id("wishlist-total"));
            Assert.IsNotNull(userWishListBtn);
            userWishListBtn.Click();

            var removeDesiredProductBtn = Browser.FindElement(Ui.By_Xpath($"//*[@id=\"wishlist\"]/div/table/tbody/tr/td[6]//a"));
            Assert.IsNotNull(removeDesiredProductBtn, "The product " + DesiredProduct + " is not present in the wish list!");
            removeDesiredProductBtn.Click();

            await Browser.Refresh();

            var productInWishlist = Browser.FindElement(Ui.By_Xpath($"//*[@id=\"wishlist\"]/div/table/tbody/tr/td[6]//a"));
            Assert.IsNotNull(productInWishlist, "The product " + DesiredProduct + " still exist in the wish list!");
        }

        [Test, Order(5)]
        public async Task DeleteAllAddressBooks()
        {
            await AddAddressBookEntry();
            await DeleteAllAddresses();
        }


        [OneTimeTearDown]
        public async Task AfterSuite()
        {
            await Browser.LogOutOpenCart();
            Browser.Dispose();
        }

        
    }
}
