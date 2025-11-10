using POC_OpenCart.UiFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace POC_OpenCart.Framework
{
    public abstract class BrowserTestFixture : TestFixture
    {
        /* Note: Do not dispose the Browser instance, so the underlying web driver can be reused between test fixtures */
        protected Browser Browser { get; }

        protected BrowserTestFixture()
        {
            Browser = Resolve<Browser>("DefaultBrowser");
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Browser.Dispose();
        }
    }
}