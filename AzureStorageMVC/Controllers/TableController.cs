using AzureStorageMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageMVC.Controllers
{
    public class TableController : Controller
    {
        private readonly CloudTableClient tableClient;

        public TableController(IConfiguration configuration)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.GetValue<string>("AzureStorage:ConnectionString"));
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public async Task<IActionResult> List()
        {
            CloudTable promotionTable = tableClient.GetTableReference("promotion");
            TableContinuationToken token = null;
            List<PromotionEntity> promotions = new List<PromotionEntity>();

            do
            {
                var queryResult = await promotionTable.ExecuteQuerySegmentedAsync<PromotionEntity>(new TableQuery<PromotionEntity>(), token);
                promotions.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            }
            while (token != null);

            return View(promotions);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(PromotionEntity promotion)
        {
            CloudTable promotionTable = tableClient.GetTableReference("promotion");

            TableOperation insertOperation = TableOperation.Insert(promotion);

            await promotionTable.ExecuteAsync(insertOperation);

            return RedirectToAction("List");
        }
    }
}