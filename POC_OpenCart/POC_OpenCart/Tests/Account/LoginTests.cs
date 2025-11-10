using System.Threading.Tasks;
using POC_OpenCart.DataHelpers;
using POC_OpenCart.Framework;
using POC_OpenCart.UiFramework;
using NUnit.Framework;
using POC_OpenCart.Configuration;

namespace POC_OpenCart.Tests.Account
{
    [TestFixture]
    public class LoginTests : BrowserTestFixture
    {
        [Test, Order(0)]
        public async Task LoginOpenCart()
        {
            await Task.Delay(2000);
            await Browser.LogInOpenCart(ConfigManager.Email, ConfigManager.Password);

            //verify user's successfull login
            var myAccountElement = Browser.FindElement(Ui.By_Xpath("//*[@id='content']/h1"));
            Assert.IsNotNull(myAccountElement, "Login is not successfull!");                      
        }            

        [TearDown]
        public async Task AfterTest()
        {
            await Browser.LogOutOpenCart();
        }
    }
}
