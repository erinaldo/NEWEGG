using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using TWNewEgg.StorageServices.Interface;
using System.Net;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.PurgeQueueAdapters.Interface;
using System.Web;

namespace TWNewEgg.StorageServices
{
    /// <summary>
    /// Azure Storage Service
    /// </summary>
    public class AzureStorageService : ICloudStorage
    {
        private CloudBlobClient _blobClient;
        private IPurgeQueueAdapters _iPurgeQueueAdapters;

        public AzureStorageService(IPurgeQueueAdapters iPurgeQueueAdapters)
        {
            this._blobClient = AzureAccount.StorageAccount.CreateCloudBlobClient();
            this._iPurgeQueueAdapters = iPurgeQueueAdapters;
        }

        public void Upload(string from, string containerName, string blobname)
        {
            // Retrieve reference to a previously created container.
            CloudBlobContainer container = this._blobClient.GetContainerReference(containerName.ToLower());
            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobname);
            string fileTypeTemp = blockBlob.Uri.Segments.LastOrDefault();
            string contentType = this.GetFileContentType(fileTypeTemp);
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(from))
            {
                blockBlob.UploadFromStream(fileStream);
                blockBlob.Properties.ContentType = contentType;
                blockBlob.SetProperties();
            }
            this.insertToPurgeTable(containerName, blobname);
        }

        public void Download(string containerName, string to, string blobname)
        {
            // Retrieve reference to a previously created container.
            CloudBlobContainer container = this._blobClient.GetContainerReference(containerName.ToLower());

            // Retrieve reference to a blob named "photo1.jpg".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobname);
            
            // Save blob contents to a file.
            using (var fileStream = System.IO.File.OpenWrite(to))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        public void Delete(string containerName, string blobname)
        {
            // Retrieve reference to a previously created container.
            CloudBlobContainer container = this._blobClient.GetContainerReference(containerName.ToLower());

            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobname);

            // Delete the blob.
            blockBlob.Delete();
        }
        
        public IEnumerable<Uri> List(string containerName)
        {
            List<Uri> UriList = new List<Uri>();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = this._blobClient.GetContainerReference(containerName.ToLower());
            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    UriList.Add(blob.Uri);
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    UriList.Add(pageBlob.Uri);
                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;
                    
                    if (directory.ListBlobs().ToList() != null)
                    {
                        var listblobs = directory.ListBlobs().ToList().Select(p=>p.Uri).ToList();
                        
                        listblobs.ForEach(q =>
                        {
                            UriList.Add(q);
                        });
                    }
                }

            }

            return UriList;
        }

        /// <summary>
        /// 新增Azure Container
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public bool CreateFolderIfNotExist(string containerName)
        {
            // Retrieve a reference to a container.
            CloudBlobContainer container = this._blobClient.GetContainerReference(containerName.ToLower());

            // Create the container if it doesn't already exist.
            bool isExist = container.CreateIfNotExists();
            container.SetPermissions(
            new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            return isExist;
        }

        /// <summary>
        /// 判斷檔案是否存在
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        public bool IsExist(string container, string blob)
        {
            bool isExist = false;
            if (string.IsNullOrEmpty(container) == true || string.IsNullOrEmpty(blob) == true)
            {
                isExist = false;
            }
            string httpStr = "https://necdn.blob.core.windows.net/" + container.ToLower() + "/" + blob;
            Uri urlCheck = new Uri(httpStr);
            WebRequest request = WebRequest.Create(urlCheck);
            request.Timeout = 15000;
            WebResponse response;
            try
            {
                response = request.GetResponse();
                isExist = true;
            }
            catch (Exception)
            {
                isExist = false;
            }

            return isExist;
        }

        private string GetFileContentType(string keyTemp = "")
        {
            string strResult = string.Empty;
            if (string.IsNullOrEmpty(keyTemp) == true)
            {
                return strResult;
            }
            if (keyTemp.IndexOf(".") >= 0)
            {
                string key = keyTemp.Split('.').ToList().LastOrDefault().ToLower();
                string contentType = System.Configuration.ConfigurationManager.AppSettings[key] == null ? string.Empty : System.Configuration.ConfigurationManager.AppSettings[key].ToString();
                strResult = contentType;
            }
            return strResult;
        }


        private PurgeQueue createInsertModel(string container, string blobname)
        {
            string url = string.Empty; 
            PurgeQueue purgeQueue = new PurgeQueue();
            purgeQueue.isPurged = 0;
            purgeQueue.CreateDate = DateTime.Now;

            char[] delimiterChars = { '/' };
            string[] urlsplit = blobname.Split(delimiterChars);
            foreach (string urlSplit in urlsplit)
            {
                //if chinese encode url
                int intChineseFrom = Convert.ToInt32("4e00", 16);
                int intChineseEnd = Convert.ToInt32("9fff", 16);
                int intCode = 0;
                int chsp = 0;
                string encodeurl = string.Empty;
                bool ch = false;
                for (int i = 0; i < urlSplit.Length; i++)
                {
                    if (ch == false)
                    {
                        intCode = Char.ConvertToUtf32(urlSplit, i);
                        if (intCode >= intChineseFrom && intCode <= intChineseEnd)
                        {
                            chsp = i;
                            ch = true;
                        }
                    }
                }
                if (ch == true)
                {
                    encodeurl = urlSplit.Substring(chsp);
                    encodeurl = HttpContext.Current.Server.UrlEncode(encodeurl);
                    url += '/' + urlSplit.Substring(0, chsp) + encodeurl;
                   
                }
                else
                {
                    url += '/' + urlSplit;
                  
                }
                
            }
            purgeQueue.URL = "/" + container.Replace("/", "") + url;
            return purgeQueue;
        }
        private void insertToPurgeTable(string container, string blobname)
        {
            PurgeQueue purgeQueue = this.createInsertModel(container, blobname);
            this._iPurgeQueueAdapters.insert(purgeQueue);
        }
        
    }
}
