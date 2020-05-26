using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorage.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class UserController : ControllerBase
    {
        private readonly BlobServiceClient blobServiceClient;

        public UserController(IConfiguration configuration)
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

        [HttpPost]
        [Route("changeAvatar")]
        public async Task<IActionResult> ChangeAvatar(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                var stream = file.OpenReadStream();
                BlobContainerClient container = blobServiceClient.GetBlobContainerClient("avatars");
                await container.UploadBlobAsync($"{Guid.NewGuid()}-{file.FileName}", stream, cancellationToken);

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        private async Task UploadToBlob(Stream file, string fileName, CancellationToken cancellationToken)
        {
            // Podany kontener Blob dla avatarów
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient("avatars");

            // Tworzenie nowego pliku blob
            await container.UploadBlobAsync($"{Guid.NewGuid()}-{fileName}.jpg", file, cancellationToken);
        }
    }
}
