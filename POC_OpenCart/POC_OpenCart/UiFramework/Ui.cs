using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace POC_OpenCart.UiFramework
{
    public static class Ui
    {
        public static By By_LoggedUser()
        {
            return By.XPath($"//mat-toolbar/div[2]/div[2]/ul/button/span");
        }

        public static By By_PageText(string text)
        {
            return By.XPath($"//p[contains(text(), '{text}')]");
        }

        public static By By_ProfileName()
        {
            return By.XPath($"//mat-toolbar/div[2]/div[2]/ul/button/span[1]");
        }

        public static By By_Id(string id)
        {
            return By.Id(id);
        }

        public static By By_Xpath(string id)
        {
            return By.XPath(id);
        }

        public static By By_LogoutButton()
        {
            return By.XPath($"//button[contains(text(), ' Logout ')]");
        }        
    }
}
