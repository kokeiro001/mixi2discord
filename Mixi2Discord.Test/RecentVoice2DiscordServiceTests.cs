using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using System.Net;

namespace Mixi2Discord.Lib.Tests
{
    [TestClass()]
    public class RecentVoice2DiscordServiceTests
    {
        [TestMethod()]
        public async Task RunAsyncTest()
        {
            // 例外とか起こさずに正常に走るかテスト
            var service = new RecentVoice2DiscordService(
                CloudStorageAccount.DevelopmentStorageAccount,
                ConfigurationManager.AppSettings["TableName"],
                ulong.Parse(ConfigurationManager.AppSettings["DiscordWebhookId"]),
                ConfigurationManager.AppSettings["DiscordWebhookToken"],
                ConfigurationManager.AppSettings["MixiEmail"],
                ConfigurationManager.AppSettings["MixiPassword"],
                new WebProxy(ConfigurationManager.AppSettings["MixiClientProxy"]));
            await service.RunAsync();
        }
    }
}