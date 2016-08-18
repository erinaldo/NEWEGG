using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.StorageServices.Interface
{
    public interface ICloudStorage
    {
        /// <summary>
        /// 上傳單一檔案
        /// </summary>
        /// <returns></returns>
        void Upload(string from, string to, string filename);

        /// <summary>
        /// 下載單一檔案
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        void Download(string from, string to, string filename);

        /// <summary>
        /// 刪除單一檔案
        /// </summary>
        /// <param name="path"></param>
        void Delete(string path, string filename);
        
        /// <summary>
        /// 列出位置下的檔案
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerable<Uri> List(string path);

        /// <summary>
        /// 如果不存在則新增資料夾
        /// </summary>
        bool CreateFolderIfNotExist(string containerName);

        /// <summary>
        /// 判斷檔案是否存在
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        bool IsExist(string container, string blob);
    }
}
