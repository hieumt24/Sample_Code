using Azure.Storage.Blobs;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Infrastructure.Services.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MatchFinder.Infrastructure.Services.Impl
{
    public class FileService : IFileService
    {
        private readonly FileAzureSettings _fileAzureSettings;

        public FileService(IOptions<FileAzureSettings> fileAzureSettings)
        {
            _fileAzureSettings = fileAzureSettings.Value;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            string containerName = _fileAzureSettings.Container;
            string connectionString = _fileAzureSettings.ConnectionString;

            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync();
            string uniqueNameFile = Guid.NewGuid().ToString() + file.FileName;
            BlobClient blobClient = containerClient.GetBlobClient(uniqueNameFile);

            //save file to blob storage and return the url of the file
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            var path = blobClient.Uri.AbsoluteUri;
            return path;
        }

        public bool IsImageFile(IFormFile file)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".jfif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!imageExtensions.Contains(extension))
            {
                throw new DataInvalidException($"{file.FileName} is not an image");
            }
            return true;
        }
    }
}