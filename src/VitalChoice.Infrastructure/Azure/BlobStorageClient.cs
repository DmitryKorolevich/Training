using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Azure;

namespace VitalChoice.Infrastructure.Azure
{
    public class BlobStorageClient : IBlobStorageClient
    {
        private readonly CloudBlobClient _blobClient;

        public BlobStorageClient(IOptions<AppOptions> appOptions)
        {
            var storageAccount = CloudStorageAccount.Parse(appOptions.Value.AzureStorage.StorageConnectionString);

            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        private async Task<IEnumerable<string>> ResolveBlobList(string containerName, string blobName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            var result = await container.CreateIfNotExistsAsync();

            if (result)
            {
                await container.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess =
                            BlobContainerPublicAccessType.Off
                    });
            }
            return container.ListBlobs(blobName + "/").Where(b => b is CloudBlockBlob).Select(b => ((CloudBlockBlob) b).Name);
        }

        private async Task<CloudBlockBlob> ResolveBlockBlob(string containerName, string blobName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            var result = await container.CreateIfNotExistsAsync();

            if (result)
            {
                await container.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess =
                            BlobContainerPublicAccessType.Off
                    });
            }
            return container.GetBlockBlobReference(blobName);
        }

        public async Task UploadBlobAsync(string containerName, string blobName, byte[] data, string contentType = null)
        {
            var blockBlob = await ResolveBlockBlob(containerName, blobName);

            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
        }

        public async Task<List<string>> GetBlobItems(string containerName, string blobPath)
        {
            return (await ResolveBlobList(containerName, blobPath)).Select(name => name.Substring(blobPath.Length + 1)).ToList();
        }

        public async Task<bool> BlobBlockExistsAsync(string containerName, string blobName)
        {
            var blockBlob = await ResolveBlockBlob(containerName, blobName);

            return await blockBlob.ExistsAsync();
        }

        public async Task<Blob> DownloadBlobBlockAsync(string containerName, string blobName)
        {
            var blockBlob = await ResolveBlockBlob(containerName, blobName);

            await blockBlob.FetchAttributesAsync();
            var fileByteLength = blockBlob.Properties.Length;
            var fileContent = new byte[fileByteLength];
            for (var i = 0; i < fileByteLength; i++)
            {
                fileContent[i] = 0x20;
            }
            await blockBlob.DownloadToByteArrayAsync(fileContent, 0);
            return new Blob()
            {
                File = fileContent,
                ContentType = blockBlob.Properties.ContentType
            };
        }

        public async Task<bool> DeleteBlobAsync(string containerName, string blobName)
        {
            var blockBlob = await ResolveBlockBlob(containerName, blobName);

            return await blockBlob.DeleteIfExistsAsync();
        }
    }
}