using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Azure;

namespace VitalChoice.Infrastructure.Azure
{
    public interface IBlobStorageClient
    {
	    Task UploadBlobAsync(string containerName, string blobName, byte[] data, string contentType = null);

	    Task<bool> BlobBlockExistsAsync(string containerName, string blobName);

        Task<List<string>> GetBlobItems(string containerName, string blobPath);

        Task<Blob> DownloadBlobBlockAsync(string containerName, string blobName);

	    Task<bool> DeleteBlobAsync(string containerName, string blobName);
	}
}
