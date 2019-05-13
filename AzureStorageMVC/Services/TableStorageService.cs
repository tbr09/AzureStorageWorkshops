using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
