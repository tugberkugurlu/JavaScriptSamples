using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMCESample.Infrastructure.Models;

namespace TinyMCESample.Infrastructure
{
    public interface IStorageManager
    {
        Task<IEnumerable<BlobItem>> GetBlobs(string containerName);
    }
}