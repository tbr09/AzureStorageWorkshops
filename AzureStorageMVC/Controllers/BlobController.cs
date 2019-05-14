using AzureStorageMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageMVC.Controllers
{
    public class BlobController : Controller
    {
        private readonly CloudBlobClient blobClient;

        public BlobController(IConfiguration configuration)
        {
            var storageAccount = CloudStorageAccount
                .Parse(configuration["AzureStorage:ConnectionString"]);
            blobClient = storageAccount.CreateCloudBlobClient();
        }

        public IActionResult List()
        {
            CloudBlobContainer container = blobClient.GetContainerReference("avatars");

            var blobList = container.ListBlobs()
                .OfType<CloudBlockBlob>()
                .Select(b => new BlobEntity()
                {
                    Name = b.Name,
                    Url = b.Uri.ToString()
                })
                .ToList();

            return View(blobList);
        }


        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile file)
        {
            var stream = file.OpenReadStream();

            CloudBlobContainer container = blobClient.GetContainerReference("avatars");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}-{file.FileName}");

            await blockBlob.UploadFromStreamAsync(stream);

            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(string fileName)
        {
            CloudBlobContainer container = blobClient.GetContainerReference("avatars");

            var blob = container.GetBlockBlobReference(fileName);

            await blob.DeleteIfExistsAsync();

            return RedirectToAction("List");
        }
    }
}
