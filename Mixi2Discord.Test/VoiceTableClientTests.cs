using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace Mixi2Discord.Lib.Tests
{
    [TestClass()]
    public class VoiceTableClientTests
    {
        [TestMethod()]
        public async Task Tableに最終観測時間を保存するテスト()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var tableName = ConfigurationManager.AppSettings["TableName"];
            var tableClient = new VoiceTableClient(storageAccount, tableName);

            await tableClient.UpdateOrInsertAsync(DateTime.Now);
        }
    }
}