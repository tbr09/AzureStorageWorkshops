using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var name = file.FileName;
            await UploadToBlob(stream, name);

            return RedirectToAction("Index");
        }

        private async Task UploadToBlob(Stream file, string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configuration["AzureStorage:ConnectionString"]);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("avatars");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}-{fileName}");

            await blockBlob.UploadFromStreamAsync(file);
        }
    }
}
