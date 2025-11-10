using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using POC_OpenCart.Configuration;

namespace POC_OpenCart.UiFramework
{
    public static class WebDriverProvider
    {
        public static ChromeDriver webDriver;

        public static IWebDriver Get()
        {
            webDriver = GetChromeDriver();

            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromMilliseconds(10000);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(2000);

            return webDriver;
        }

        private static ChromeDriver GetChromeDriver()
        {
            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArgument("--start-maximized");
            chromeOptions.AddArguments("--no-sandbox");

            /* Replace with your own user */
            //chromeOptions.AddArguments("user-data-dir=C:\\Users\\p.d.anghel\\AppData\\Local\\Google\\Chrome\\User Data");
            //chromeOptions.AddArguments("user-data-dir=C:\\Users\\apaul\\AppData\\Local\\Google\\Chrome\\User Data");

            chromeOptions.AddArguments("--disable-extensions");
            chromeOptions.AddArguments("--proxy-server='direct://'");
            chromeOptions.AddArguments("--proxy-bypass-list=*");
            chromeOptions.AddArguments("--start-maximized");
            chromeOptions.AddArguments("--incognito");

            if (ConfigManager.HeadlessMode)
            {
                chromeOptions.AddArguments("--headless");
            }

            chromeOptions.AddArguments("--disable-gpu");
            chromeOptions.AddArguments("--no-sandbox");
            chromeOptions.AddArguments("--disable-dev-shm-usag");
            chromeOptions.AddArguments("--ignore-certificate-errors");

            return new ChromeDriver(chromeOptions);
        }       
    }
}