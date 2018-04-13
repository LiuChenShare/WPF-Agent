﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Titanium.Web.Proxy.Network.WinAuth;

namespace Titanium.Web.Proxy.UnitTests
{
    [TestClass]
    public class WinAuthTests
    {
        [TestMethod]
        public void Test_Acquire_Client_Token()
        {
            string token = WinAuthHandler.GetInitialAuthToken("mylocalserver.com", "NTLM", Guid.NewGuid());
            Assert.IsTrue(token.Length > 1);
        }
    }
}
