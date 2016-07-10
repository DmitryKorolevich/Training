using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly HashSet<string> _initializedContainers;

        public BlobStorageClient(IOptions<AppOptions> appOptions)
        {
            var storageAccount = CloudStorageAccount.Parse(appOptions.Value.AzureStorage.StorageConnectionString);

            var blobServicePoint = ServicePointManager.FindServicePoint(storageAccount.BlobEndpoint);
            blobServicePoint.ConnectionLimit = 100;

            _blobClient = storageAccount.CreateCloudBlobClient();
            _initializedContainers = new HashSet<string>();
        }

        private async Task<IEnumerable<string>> GetBlobList(string containerName, string blobName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            var needInitialization = false;
            lock (_initializedContainers)
            {
                if (!_initializedContainers.Contains(containerName))
                {
                    _initializedContainers.Add(containerName);
                    needInitialization = true;
                }
            }
            if (needInitialization)
            {
                if (await container.CreateIfNotExistsAsync())
                {
                    await container.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Off
                        });
                }
                return Enumerable.Empty<string>();
            }
            return container.ListBlobs(blobName + "/").Where(b => b is CloudBlockBlob).Select(b => ((CloudBlockBlob) b).Name);
        }

        private async Task<CloudBlockBlob> GetBlockReference(string containerName, string blobName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            var needInitialization = false;
            lock (_initializedContainers)
            {
                if (!_initializedContainers.Contains(containerName))
                {
                    _initializedContainers.Add(containerName);
                    needInitialization = true;
                }
            }
            if (needInitialization)
            {
                if (await container.CreateIfNotExistsAsync())
                {
                    await container.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Off
                        });
                }
            }
            return container.GetBlockBlobReference(blobName);
        }

        public async Task UploadBlobAsStringAsync(string containerName, string blobName, string data, string contentType = null)
        {
            var blockBlob = await GetBlockReference(containerName, blobName);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadTextAsync(data);
        }

        public async Task UploadBlobAsync(string containerName, string blobName, byte[] data, string contentType = null)
        {
            var blockBlob = await GetBlockReference(containerName, blobName);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
        }

        public async Task<List<string>> GetBlobItems(string containerName, string blobPath)
        {
            return (await GetBlobList(containerName, blobPath)).Select(name => name.Substring(blobPath.Length + 1)).ToList();
        }

        public async Task<bool> BlobBlockExistsAsync(string containerName, string blobName)
        {
            var blockBlob = await GetBlockReference(containerName, blobName);

            return await blockBlob.ExistsAsync();
        }

        public async Task<Blob> DownloadBlobBlockAsync(string containerName, string blobName)
        {
            var blockBlob = await GetBlockReference(containerName, blobName);

            await blockBlob.FetchAttributesAsync();
            var fileByteLength = blockBlob.Properties.Length;
            var fileContent = new byte[fileByteLength];
            await blockBlob.DownloadToByteArrayAsync(fileContent, 0);
            return new Blob
            {
                File = fileContent,
                ContentType = blockBlob.Properties.ContentType
            };
        }

        public async Task<string> DownloadBlobAsStringAsync(string containerName, string blobName)
        {
            var blockBlob = await GetBlockReference(containerName, blobName);
            return await blockBlob.DownloadTextAsync();
        }

        public async Task<bool> DeleteBlobAsync(string containerName, string blobName)
        {
            var blockBlob = await GetBlockReference(containerName, blobName);
            return await blockBlob.DeleteIfExistsAsync();
        }
    }
}