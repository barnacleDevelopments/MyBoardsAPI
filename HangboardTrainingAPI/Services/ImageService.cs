using Azure;
using Azure.Storage.Blobs;

namespace HangboardTrainingAPI.Services
{
    public class ImageService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public ImageService(IConfiguration config, ILogger<ILogger> logger)
        {

            _config = config;
            _logger = logger;
        }

        public async Task<string> UploadImage(IFormFile file, int HangboardId)
        {
       
            var UUID = Guid.NewGuid().ToString();

            var blobFileName = $"{HangboardId}_{UUID}.jpg";

            // I container does not exist create blob container
            var container = await CreateBlobContainer();

            if(container == null)
            {
                return "";
            }

            // Upload blob to container
            BlobClient blobClient = container.GetBlobClient(blobFileName);
            await blobClient.UploadAsync(file.OpenReadStream());

            return blobClient.Uri.AbsoluteUri;
        }

        private async Task<BlobContainerClient?> CreateBlobContainer()
        {
            try
            {
                var azureStorageConnectionString = _config["ConnectionStrings:BlobStorage"];
                BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
                string containerName = "hangboard-images";
                BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);

                // check if container exists 
                if (!await container.ExistsAsync())
                {
                    return await blobServiceClient.CreateBlobContainerAsync(containerName);
                }

                return container;

            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Something went wrong inside the DeleteAsync method inside the WorkoutController: {ex}");
            }

            return null;
        }
    }
}
