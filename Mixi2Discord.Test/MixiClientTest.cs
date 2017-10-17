using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixi2Discord.Lib;

namespace Mixi2Discord.Test
{
    [TestClass]
    public class MixiClientTest
    {
        WebProxy Proxy => new WebProxy(ConfigurationManager.AppSettings["MixiClientProxy"]);

        [TestMethod]
        public async Task LoginTest()
        {
            var client = new MixiClient(Proxy);
            await client.LoginAsync(ConfigurationManager.AppSettings["MixiEmail"], ConfigurationManager.AppSettings["MixiPassword"]);
        }

        [TestMethod]
        public async Task GetRecentVoicesTest()
        {
            var client = new MixiClient(Proxy);
            await client.LoginAsync(ConfigurationManager.AppSettings["MixiEmail"], ConfigurationManager.AppSettings["MixiPassword"]);

            await client.GetRecentVoicesAsync();
        }
    }
}
