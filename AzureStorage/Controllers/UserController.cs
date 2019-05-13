using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorage.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public UserController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("changeAvatar")]
        public async Task<IActionResult> ChangeAvatar(IFormFile file)
        {
            try
            {
                var stream = file.OpenReadStream();
                var name = file.FileName;
                await UploadToBlob(stream, name);

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
        }

        private async Task UploadToBlob(Stream file, string fileName)
        {
            // Dane dostępowe 
            StorageCredentials storageCredentials = new StorageCredentials(configuration["AzureStorage:AccountName"], configuration["AzureStorage:AccountKey"]);

            // Obiekt reprezentujący konto Storage
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            // Obiekt reprezentujący serwis Blob w obrębie wybranego konta Storage
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Podany kontener Blob dla avatarów
            CloudBlobContainer container = blobClient.GetContainerReference("avatars");

            // Tworzenie nowego pliku blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}-{fileName}.jpg");

            // Wgrywanie podanego pliku do bloba
            await blockBlob.UploadFromStreamAsync(file);
        }
    }
}
