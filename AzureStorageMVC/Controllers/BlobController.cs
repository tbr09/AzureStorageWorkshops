using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorageMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageMVC.Controllers
{
    public class BlobController : Controller
    {
        private readonly BlobServiceClient blobServiceClient;

        public BlobController(IConfiguration configuration)
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

        public async Task<IActionResult> ListAsync()
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient("avatars");

            var blobs = new List<BlobClient>();

            await foreach (BlobItem blob in container.GetBlobsAsync())
            {
                var blobClient = container.GetBlobClient(blob.Name);
                blobs.Add(blobClient);
            }

            var result = blobs.Select(b => new BlobEntity()
            {
                Name = b.Name,
                Url = b.Uri.ToString()
            });

            return View(result);
        }


        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile file, CancellationToken cancellationToken)
        {
            var stream = file.OpenReadStream();

            BlobContainerClient container = blobServiceClient.GetBlobContainerClient("avatars");

            await container.UploadBlobAsync($"{Guid.NewGuid()}-{file.FileName}", stream, cancellationToken);

            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(string fileName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient("avatars");

            await container.DeleteBlobAsync(fileName);

            return RedirectToAction("List");
        }
    }
}
