using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Transfer.Azure;

namespace VitalChoice.Infrastructure.Azure
{
    public class BlobStorageClient: IBlobStorageClient
    {
	    private CloudBlobClient _blobClient;

	    public BlobStorageClient(IOptions<AppOptions> appOptions)
	    {
		    var storageAccount = CloudStorageAccount.Parse(appOptions.Value.AzureStorage.StorageConnectionString);

		    _blobClient = storageAccount.CreateCloudBlobClient();
		   
	    }

	    private async Task<CloudBlockBlob> ResolveBlob(string containerName, string blobName)
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
			var blockBlob = await ResolveBlob(containerName, blobName);

			blockBlob.Properties.ContentType = contentType;
			await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
	    }

		public async Task<bool> BlobExistsAsync(string containerName, string blobName)
	    {
			var blockBlob = await ResolveBlob(containerName, blobName);

			return await blockBlob.ExistsAsync();
	    }

	    public async Task<Blob> DownloadBlobAsync(string containerName, string blobName)
	    {
			var blockBlob = await ResolveBlob(containerName, blobName);

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
			var blockBlob = await ResolveBlob(containerName, blobName);

			return await blockBlob.DeleteIfExistsAsync();
	    }
    }
}
