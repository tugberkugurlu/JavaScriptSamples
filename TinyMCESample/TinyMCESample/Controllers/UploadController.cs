using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using TinyMCESample.Infrastructure.StreamProviders;

namespace TinyMCESample.Controllers
{
    public class UploadController : ApiController
    {
        public async Task<IEnumerable<string>> PostUpload(string category)
        {
            // Check whether it is an HTML form file upload request
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                // return UnsupportedMediaType response back if not
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType));
            }

            //MultipartFormDataStreamProvider multipartFormDataStreamProvider = new CustomMultipartFormDataStreamProvider(@"C:\Trash");
            //await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);
            //return multipartFormDataStreamProvider.FileData.Select(entry => entry.LocalFileName);

            CloudBlobContainer container = await GetCloudBlobContainer(category.ToSlug());
            AzureBlobStorageMultipartProvider provider = new AzureBlobStorageMultipartProvider(container);
            await Request.Content.ReadAsMultipartAsync(provider);

            return provider.AzureBlobs.Select(entry => entry.Location);
        }

        // FFS, nothing is thread-safe!
        private async Task<CloudBlobContainer> GetCloudBlobContainer(string containerName)
        {
            const string StorageConnStrKey = "StorageConnectionString";

            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(ConfigurationManager.AppSettings[StorageConnStrKey].ToString(), out storageAccount))
            {
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

                // Check container names: http://msdn.microsoft.com/en-us/library/dd135715.aspx

                if (await blobContainer.CreateIfNotExistsAsync())
                {
                    await blobContainer.SetPermissionsAsync(new BlobContainerPermissions 
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
                }

                return blobContainer;
            }
            else
            {
                throw new InvalidOperationException("Couldn't parse the storage connection string!");
            }
        }
    }

    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string rootPath) : base(rootPath) 
        {
        }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers != null && headers.ContentDisposition != null) 
            {
                return Path.GetFileName(headers.ContentDisposition.FileName.Trim('"'));
            }

            return base.GetLocalFileName(headers);
        }
    }

    public abstract class MultipartBlobStreamProvider : MultipartStreamProvider
    {
        private const int MinBufferSize = 1;
        private const int DefaultBufferSize = 0x1000;

        private string _container;
        private int _bufferSize = DefaultBufferSize;

        private Collection<MultipartFileData> _fileData = new Collection<MultipartFileData>();

        public MultipartBlobStreamProvider(string container) : this(container, DefaultBufferSize)
        {
        }

        public MultipartBlobStreamProvider(string container, int bufferSize)
        {
            if (container == null)
            {
                throw new ArgumentNullException("rootPath");
            }

            if (bufferSize < MinBufferSize)
            {
                throw new ArgumentOutOfRangeException("bufferSize", bufferSize, string.Format("Minimum value should be '{0}'.", MinBufferSize));
            }

            _container = Path.GetFullPath(container);
            _bufferSize = bufferSize;
        }

        protected abstract Stream GetStream(string container, string filename, int bufferSize);

        public sealed override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            string filename = GetLocalFileName(headers);
            return GetStream(_container, filename, _bufferSize);
        }

        public virtual string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            return string.Format(CultureInfo.InvariantCulture, "BodyPart_{0}", Guid.NewGuid());
        }
    }

    public class MultipartFileSystemStreamProvider : MultipartBlobStreamProvider
    {
        public MultipartFileSystemStreamProvider(string container) : base(container)
        {
        }

        protected override Stream GetStream(string container, string filename, int bufferSize)
        {
            string localFilePath;
            try
            {
                localFilePath = Path.Combine(container, Path.GetFileName(filename));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not determine a valid file name for the multipart body part.", e);
            }

            return File.Create(localFilePath, bufferSize, FileOptions.Asynchronous);
        }
    }
}