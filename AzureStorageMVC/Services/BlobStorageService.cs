using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageMVC.Services
{
    public class BlobStorageService 
    {
        private readonly BlobServiceClient blobServiceClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var blobServiceEndpoint = configuration.GetValue<string>("AzureStorage:BlobServiceEndpoint");
            var storageAccountName = configuration.GetValue<string>("AzureStorage:AccountName");
            var storageAccountKey = configuration.GetValue<string>("AzureStorage:AccountKey");

            // First option - credentials
            StorageSharedKeyCredential accountCredentials = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);
            blobServiceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), accountCredentials);

            // Second option - connection string
            //var storageConnectionString = configuration.GetValue<string>("AzureStorage:ConnectionString");
            //blobServiceClient = new BlobServiceClient(storageConnectionString);
        }

        public async Task UploadToBlob(Stream file, string fileName, string containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.UploadBlobAsync(fileName, file);
        }

        public async Task DeleteBlob(string fileName, string containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.DeleteBlobAsync(fileName);
        }
    }
}
