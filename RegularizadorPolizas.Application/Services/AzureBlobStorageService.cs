using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegularizadorPolizas.Application.Interfaces;

namespace RegularizadorPolizas.Infrastructure.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly int _urlExpirationHours;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public AzureBlobStorageService(
            BlobServiceClient blobServiceClient,
            ILogger<AzureBlobStorageService> logger)
        {
            _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _containerName = "poliza-pdfs";
            _urlExpirationHours = 24;
        }

        public async Task<string> SavePdfAsync(IFormFile file, int clientId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is null or empty");

                // Crear nombre único para el archivo
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"client-{clientId}/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}{fileExtension}";

                _logger.LogInformation("Saving PDF file {FileName} as {UniqueFileName}", file.FileName, uniqueFileName);

                // Obtener container cliente
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

                // Crear container si no existe
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                // Subir archivo con metadatos
                var blobClient = containerClient.GetBlobClient(uniqueFileName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "application/pdf"
                };

                var metadata = new Dictionary<string, string>
                {
                    ["ClientId"] = clientId.ToString(),
                    ["OriginalFileName"] = file.FileName,
                    ["UploadDate"] = DateTime.UtcNow.ToString("O"),
                    ["FileSize"] = file.Length.ToString()
                };

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders,
                    Metadata = metadata
                });

                // Generar URL con SAS token temporal
                var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read,
                    DateTimeOffset.UtcNow.AddHours(_urlExpirationHours));

                _logger.LogInformation("PDF file saved successfully. URL expires in {Hours} hours", _urlExpirationHours);

                return sasUri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving PDF file {FileName} for client {ClientId}", file?.FileName, clientId);
                throw;
            }
        }

        public async Task<string> GetPdfUrlAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                var exists = await blobClient.ExistsAsync();
                if (!exists.Value)
                {
                    _logger.LogWarning("PDF file {FileName} not found", fileName);
                    return null;
                }

                // Generar nueva URL con SAS token
                var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read,
                    DateTimeOffset.UtcNow.AddHours(_urlExpirationHours));

                return sasUri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting URL for PDF file {FileName}", fileName);
                throw;
            }
        }

        public async Task<byte[]> GetPdfBytesAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                var response = await blobClient.DownloadContentAsync();
                return response.Value.Content.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading PDF file {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeletePdfAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                var response = await blobClient.DeleteIfExistsAsync();

                if (response.Value)
                {
                    _logger.LogInformation("PDF file {FileName} deleted successfully", fileName);
                }
                else
                {
                    _logger.LogWarning("PDF file {FileName} was not found for deletion", fileName);
                }

                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PDF file {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> PdfExistsAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                var response = await blobClient.ExistsAsync();
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if PDF file {FileName} exists", fileName);
                throw;
            }
        }

        public async Task<long> GetPdfSizeAsync(string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                var properties = await blobClient.GetPropertiesAsync();
                return properties.Value.ContentLength;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting size of PDF file {FileName}", fileName);
                throw;
            }
        }
    }
}