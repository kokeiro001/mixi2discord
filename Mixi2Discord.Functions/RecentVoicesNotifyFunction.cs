using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Mixi2Discord.Lib;

namespace Mixi2Discord.Functions
{
    public static class NotifyFunction
    {
        [FunctionName("RecentVoicesNotifyTimerTrigger")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                var service = new RecentVoice2DiscordService(
                    GetDefaultStorageAccount(),
                    CloudConfigurationManager.GetSetting("TableName"),
                    ulong.Parse(CloudConfigurationManager.GetSetting("DiscordWebhookId")),
                    CloudConfigurationManager.GetSetting("DiscordWebhookToken"),
                    CloudConfigurationManager.GetSetting("MixiEmail"),
                    CloudConfigurationManager.GetSetting("MixiPassword"),
                    new WebProxy(CloudConfigurationManager.GetSetting("MixiClientProxy")));
                await service.RunAsync(x => log.Info(x));
            }
            catch (Exception e)
            {
                log.Error(e.Message + " " + e.StackTrace, e);
                throw;
            }
        }


        public static CloudStorageAccount GetDefaultStorageAccount() =>
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));
    }
}