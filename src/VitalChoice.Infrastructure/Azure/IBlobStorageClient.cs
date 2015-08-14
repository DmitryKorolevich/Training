using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Azure;

namespace VitalChoice.Infrastructure.Azure
{
    public interface IBlobStorageClient
    {
	    Task UploadBlobAsync(string containerName, string blobName, byte[] data, string contentType = null);

	    Task<bool> BlobExistsAsync(string containerName, string blobName);

	    Task<Blob> DownloadBlobAsync(string containerName, string blobName);

	    Task<bool> DeleteBlobAsync(string containerName, string blobName);
	}
}
