using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Azure;

namespace VitalChoice.Infrastructure.Azure
{
    public interface IBlobStorageClient
    {
	    Task UploadBlobAsync(string blobname, byte[] data);

		IList<string> ListBlobs(string prefix);

	    Task<bool> BlobExistsAsync(string blobname);

	    Task<Blob> DownloadBlobAsync(string blobname);
    }
}
