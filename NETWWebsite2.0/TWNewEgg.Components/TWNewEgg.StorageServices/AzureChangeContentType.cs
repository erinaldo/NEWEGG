using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.StorageServices.Interface;

namespace TWNewEgg.StorageServices
{
    public class AzureChangeContentType : IAure
    {
        private CloudBlobClient _blobClient;

        public AzureChangeContentType()
        {
            this._blobClient = AzureAccount.StorageAccount.CreateCloudBlobClient();
        }

        public List<string> changeFileContentTypeFileByBatch(string containerName, string direction, string contentType = "image/jpeg")
        {
            List<string> result = new List<string>();
            CloudBlobContainer container = this._blobClient.GetContainerReference(containerName.ToLower());
            var directory = container.GetDirectoryReference(direction).ListBlobs(true);
            foreach (var blob in directory)
            {
                CloudBlockBlob item = (CloudBlockBlob)blob;
                try
                {
                    ((CloudBlockBlob)blob).Properties.ContentType = contentType;
                    ((CloudBlockBlob)blob).SetProperties();
                    result.Add(item.Uri.ToString() + "; [Success]");
                }
                catch (Exception ex)
                {
                    result.Add(item.Uri.ToString() + "; [Error] : " + ex.ToString());
                }
            }
            return result;
        }
        public void ChangeFileContentTypeForBlob(string containerName, string contentType, string blob)
        {
            CloudBlobContainer container = this._blobClient.GetContainerReference(containerName.ToLower());
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blob);
            blockBlob.Properties.ContentType = contentType;
            blockBlob.SetProperties();
        }
    }
}
