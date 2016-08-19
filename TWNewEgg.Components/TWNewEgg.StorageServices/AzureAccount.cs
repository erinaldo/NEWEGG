using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.StorageServices
{
    public class AzureAccount
    {
        private static string storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
        private static CloudStorageAccount ECStorageAccount { get; set; }
        public static CloudStorageAccount StorageAccount
        {
            get
            {
                setAccount(ECStorageAccount);
                return ECStorageAccount;
            }
        }

        private static void setAccount(CloudStorageAccount account)
        {
            try
            {
                CloudStorageAccount newAccount;
                if (needReNew())
                {
                    storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
                    ECStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
                }
                else
                {
                    ECStorageAccount = account;
                }
            }
            catch { throw; }
        }

        private static bool needReNew()
        {
            return storageConnectionString != ConfigurationManager.AppSettings["StorageConnectionString"] || ECStorageAccount == null;
        }
    }
}
