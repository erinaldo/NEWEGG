using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.StorageServices.Interface;
using TWNewEgg.StorageServices.Model;


namespace TWNewEgg.StorageServices
{
    public class StorageService : ICloudStorageAdapter
    {
        private ICloudStorage _storageService;
        private ICDNAdapter _cdnAdapter;
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public StorageService(ICloudStorage storageService, ICDNAdapter cdnAdapter)
        {
            this._storageService = storageService;
            this._cdnAdapter = cdnAdapter;
        }

        public void Upload(string path)
        {
            try
            {
                string imgPath = System.Configuration.ConfigurationManager.AppSettings["ImgLocal"];
                string ImgContainer = System.Configuration.ConfigurationManager.AppSettings["ImgContainer"];
                string TargetPath = "";
                List<string> paths = new List<string>();
                if (!Directory.Exists(path))
                {
                    throw new Exception(string.Format("此路徑不存在：{0}，請輸入正確路徑!!", path));
                }

                var folder = new DirectoryInfo(path);
                var files = folder.GetFiles();
                foreach (var fileInfo in files)
                {
                    string blobName = fileInfo.FullName;
                    if (blobName.Contains(imgPath))
                    {
                        TargetPath = blobName.Substring(blobName.IndexOf(imgPath) + imgPath.Length);
                    }
                    else
                    {
                        throw new InvalidOperationException("檔名格式不正確，需在AppSettings: ImgLocal所設定路徑下");
                    }

                    AzureBlobUpload UploadData = new AzureBlobUpload();
                    UploadData.container = ImgContainer;
                    UploadData.absolutePath = fileInfo.FullName;
                    UploadData.blob = "pic" + TargetPath;

                    UploadData.blob = this.getBlobName(UploadData.blob);
                    this.FileUploadProcess(UploadData);
                    paths.Add("/" + ImgContainer + "/" + UploadData.blob);

                    //儲存成功後，刪除圖片
                    DeleteLocal(UploadData.absolutePath);
                }

                //清除CDN上的content
                this._cdnAdapter.Purge(paths);

                //取得子資料夾
                var subFolders = folder.GetDirectories();
                foreach (var directoryInfo in subFolders)
                {
                    Upload(directoryInfo.FullName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        public void DeleteLocal(string path)
        {
            //儲存成功後，刪除圖片
            if (System.IO.File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (System.IO.IOException e)
                {
                    logger.Error(e.ToString());
                }
                catch (System.Exception e)
                {
                    throw;
                }
            }
        }

        private void FileUploadProcess(AzureBlobUpload azureUpload)
        {
            try
            {
                bool bool_CreateFolderIfNotExist = this._storageService.CreateFolderIfNotExist(azureUpload.container);
                this._storageService.Upload(azureUpload.absolutePath, azureUpload.container, azureUpload.blob);
            }
            catch (StorageException s_error)
            {
                throw new StorageException("Azure Storage Service出現錯誤", s_error);
            }
            catch (Exception error)
            {
                throw;
            }
        }
        private string getBlobName(string filepath)
        {
            int ind = filepath.LastIndexOf(@"\");
            string path = filepath.Substring(0, ind).ToLower();
            string filename = filepath.Substring(ind + 1);
            string blobPath = path + @"\" + filename;
            string blobname = blobPath.Replace('\\', '/');
            return blobname;
        }
    }
}
