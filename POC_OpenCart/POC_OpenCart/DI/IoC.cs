using OpenQA.Selenium;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using POC_OpenCart.UiFramework;

namespace POC_OpenCart.DI
{
    public static class IoC
    {
        #region Properties
        public static IUnityContainer Container { get; }
        #endregion


        #region .ctor
        static IoC()
        {
            Container = new UnityContainer();
            ConfigureContainer();
        }
        #endregion


        #region Lifetime manager
        private static TransientLifetimeManager Transient => new TransientLifetimeManager();
        #endregion


        #region Configuration
        private static void ConfigureContainer()
        {
            Container.RegisterFactory<IWebDriver>((x) => WebDriverProvider.Get(), Transient);
            Container.RegisterFactory<Browser>("DefaultBrowser", (c) => new Browser(c.Resolve<IWebDriver>()), Transient);
        }
        #endregion
    }
}