using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Mixi2Discord.Lib
{
    public class VoiceTableClient
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly string tableName;

        readonly CloudTable table;

        public VoiceTableClient(CloudStorageAccount storageAccount, string tableName)
        {
            this.storageAccount = storageAccount;
            this.tableName = tableName;
            table = GetCloudTableAsync().Result;
        }

        private async Task<CloudTable> GetCloudTableAsync()
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public DateTime? GetLastObservedAt()
        {
            var filter1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "partitionKey");

            var query = new TableQuery<VoiceEntity>().Where(filter1);
            var item = table.ExecuteQuery(query).FirstOrDefault();
            return item?.LastObservedAt;
        }

        public async Task UpdateOrInsertAsync(DateTime lastObservedAt)
        {
            var entity = new VoiceEntity
            {
                PartitionKey = "partitionKey",
                RowKey = "rowKey",
                LastObservedAt = lastObservedAt,
            };

            var op = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(op);
        }
    }

    public class VoiceEntity : TableEntity
    {
        public DateTime LastObservedAt { get; set; }

        public VoiceEntity()
        {
        }
    }
}
