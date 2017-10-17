using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using Microsoft.WindowsAzure.Storage;
using Mixi2Discord.Lib.Models;

namespace Mixi2Discord.Lib
{
    public class RecentVoice2DiscordService
    {
        readonly CloudStorageAccount storageAccount;
        readonly string tableName;
        readonly ulong discordWebhookId;
        readonly string discordWebhookToken;
        readonly string mixiEmail;
        readonly string mixiPassword;
        readonly WebProxy mixiClientProxy;

        public RecentVoice2DiscordService(
            CloudStorageAccount storageAccount,
            string tableName,
            ulong discordWebhookId,
            string discordWebhookToken,
            string mixiEmail,
            string mixiPassword,
            WebProxy mixiClientProxy)
        {
            this.storageAccount = storageAccount;
            this.tableName = tableName;
            this.discordWebhookId = discordWebhookId;
            this.discordWebhookToken = discordWebhookToken;
            this.mixiEmail = mixiEmail;
            this.mixiPassword = mixiPassword;
            this.mixiClientProxy = mixiClientProxy;
        }

        Action<string> logAction;
        void Log(string text)
        {
            logAction?.Invoke(text);
        }

        public async Task RunAsync(Action<string> logAction = null)
        {
            this.logAction = logAction;

            // 最後に観測した日時をTableから取得する。これより新しいつぶやきのみ通知する。、
            var lastObservedAt = GetLastObservedAt();
            Log($"lastObservedAt ={lastObservedAt }");

            var voiceItems = await GetVoiceItemsAsync();

            var newVoiceItems = voiceItems.Where(x => x.PostTime > lastObservedAt);
            var newComments = voiceItems
                                .SelectMany(x => x.Response)
                                .Where(x => x.PostTime > lastObservedAt);
            
            var client = new DiscordWebhookClient(discordWebhookId, discordWebhookToken);
            
            // 新着ボイスを通知
            await NotifyVoices(newVoiceItems, client);

            // 新着コメントを通知
            await NotifyComments(newComments, client);

            await UpdateLastObservedAtAsync();
        }

        private DateTime GetLastObservedAt()
        {
            var tableClient = new VoiceTableClient(storageAccount, tableName);
            var lastObservedAt = tableClient.GetLastObservedAt();
            if (lastObservedAt.HasValue)
            {
                return lastObservedAt.Value;
            }
            // １時間前に取得したことにする＝１時間分のつぶやきを取得するようにする
            return DateTime.UtcNow.AddHours(-1);
        }

        private async Task UpdateLastObservedAtAsync()
        {
            var tableClient = new VoiceTableClient(storageAccount, tableName);
            await tableClient.UpdateOrInsertAsync(DateTime.UtcNow);
        }

        private async Task<IEnumerable<VoiceModel>> GetVoiceItemsAsync()
        {
            var mixiClient = new MixiClient(mixiClientProxy);
            await mixiClient.LoginAsync(mixiEmail, mixiPassword);
            return await mixiClient.GetRecentVoicesAsync();
        }

        private async Task NotifyVoices(IEnumerable<VoiceModel> newVoiceItems, DiscordWebhookClient client)
        {
            foreach (var voice in newVoiceItems)
            {
                Log("voice.PostTime=" + voice.PostTime);
                var embedBuilder = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        Name = voice.NickName,
                        Url = voice.Url,
                    },
                    Description = voice.Voice
                };

                await client.SendMessageAsync("", embeds: new Embed[] { embedBuilder.Build() });
                await Task.Delay(100);
            }
        }
        private async Task NotifyComments(IEnumerable<VoiceResponse> newComments, DiscordWebhookClient client)
        {
            foreach (var comment in newComments)
            {
                Log("comment.PostTime=" + comment.PostTime);
                var embedBuilder = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        Name = comment.NickName,
                        Url = comment.Parent.Url,
                    },
                    Description = comment.Voice,
                };
                await client.SendMessageAsync("", embeds: new Embed[] { embedBuilder.Build() });
                await Task.Delay(100);
            }
        }


    }
}
