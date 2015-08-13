#if DNX451
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VitalChoice.Domain.Entities.Azure;
using VitalChoice.Domain.Entities.Options;

namespace VitalChoice.Infrastructure.Azure
{
    public class BlobStorageClient: IBlobStorageClient
    {
	    private readonly AzureStorage _appOptions;
	    private CloudBlobClient _blobClient;
	    private CloudBlobContainer _container;

	    public BlobStorageClient(IOptions<AppOptions> appOptions)
	    {
			_appOptions = appOptions.Options.AzureStorage;
		    var storageAccount = CloudStorageAccount.Parse(_appOptions.StorageConnectionString);

			_blobClient = storageAccount.CreateCloudBlobClient();
			_container = _blobClient.GetContainerReference(_appOptions.CustomerContainerName);

			_container.CreateIfNotExists();
			_container.SetPermissions(
				new BlobContainerPermissions
				{
					PublicAccess =
						BlobContainerPublicAccessType.Off
				});
		}

	    public async Task UploadBlobAsync(string blobname, byte[] data)
	    {
		    var blockBlob =_container.GetBlockBlobReference(blobname);

		    using (var stream = new MemoryStream(data))
		    {
				await blockBlob.UploadFromStreamAsync(stream);
			}
	    }

	    public IList<string> ListBlobs(string prefix)
	    {
		    var blobs = _container.ListBlobs(prefix);

		    return blobs.Select(x => x.Uri.ToString()).ToList();
	    }

	    public async Task<bool> BlobExistsAsync(string blobname)
	    {
			var blockBlob = _container.GetBlockBlobReference(blobname);

		    return await blockBlob.ExistsAsync();
	    }

	    public async Task<Blob> DownloadBlobAsync(string blobname)
	    {
			var blockBlob = _container.GetBlockBlobReference(blobname);

			blockBlob.FetchAttributes();
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
    }
}

#endif
