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

        public BlobStorageService(IConfiguration configuration)
        {
            storageAccount = CloudStorageAccount.Parse(configuration["AzureStorage:ConnectionString"]);
            blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task UploadToBlob(Stream file, string fileName, string containerName)
        {
            var blobContainer = blobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference($"{Guid.NewGuid()}-{fileName}");

            await blockBlob.UploadFromStreamAsync(file);
        }

        public async Task DeleteBlob(string fileName, string containerName)
        {
            var blobContainer = blobClient.GetContainerReference(containerName);

            var blob = blobContainer.GetBlockBlobReference(fileName);

            await blob.DeleteIfExistsAsync();
        }
    }
}
