using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageMVC.Services
{
    public class BlobStorageService 
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudBlobClient blobClient;
        private readonly CloudBlobContainer blobContainer;

        public BlobStorageService(IConfiguration configuration)
        {
            storageAccount = CloudStorageAccount.Parse(configuration["AzureStorage:ConnectionString"]);
            blobClient = storageAccount.CreateCloudBlobClient();
            blobContainer = blobClient.GetContainerReference("avatars");
        }

        private async Task UploadToBlob(Stream file, string fileName)
        {
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference($"{Guid.NewGuid()}-{fileName}");
            await blockBlob.UploadFromStreamAsync(file);
        }
    }
}
