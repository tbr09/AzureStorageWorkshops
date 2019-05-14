using AzureStorageMVC.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageMVC.Services
{
    public class TableStorageService
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;

        public TableStorageService(IConfiguration configuration)
        {
            storageAccount = CloudStorageAccount.Parse(configuration.GetValue<string>("AzureStorage:ConnectionString"));
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public async Task UploadPromotion(PromotionEntity promotion)
        {
            CloudTable promotionTable = tableClient.GetTableReference("promotion");

            TableOperation insertOperation = TableOperation.Insert(promotion);

            await promotionTable.ExecuteAsync(insertOperation);
        }

        public async Task UploadBatchPromotion(IEnumerable<PromotionEntity> promotions)
        {
            CloudTable promotionTable = tableClient.GetTableReference("promotion");

            TableBatchOperation batchOperation = new TableBatchOperation();

            foreach(var promotion in promotions)
            {
                batchOperation.Insert(promotion);
            }

            await promotionTable.ExecuteBatchAsync(batchOperation);
        }
    }
}
