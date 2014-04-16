using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TinyMCESample.Infrastructure.Models;

namespace TinyMCESample.Infrastructure
{
    public class AzureStorageManager : IStorageManager
    {
        public AzureStorageManager()
        {
        }

        public Task<IEnumerable<BlobItem>> GetBlobs(string containerName)
        {
            throw new NotImplementedException();
        }
    }
}