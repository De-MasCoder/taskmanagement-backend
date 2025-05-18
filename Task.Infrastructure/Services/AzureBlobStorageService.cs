using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

using TaskManagement.Application.Interfaces;

namespace TaskManagement.Infrastructure.Services
{
    public class AzureBlobStorageService: IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:BlobStorageConnectionString"];
            var containerName = configuration["AzureStorage:ContainerName"];
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default, string folder = "")
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken);
            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });
            return blobClient.Uri.ToString();
        }
    }

}
