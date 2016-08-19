using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TWNewEgg.FileServices.Interface;

namespace TWNewEgg.FileServices
{
    public class FileService : IFileService
    {
        public void fileUpload(System.Web.HttpPostedFileBase file, int? chunk, string name, string uploadPath)
        {
            uploadOriginalImage(chunk, file, name, uploadPath);
        }

        /// <summary>
        /// 上傳原始圖片
        /// </summary>
        /// <param name="model"></param>
        private void uploadOriginalImage(int? chunk, HttpPostedFileBase file, string name, string subpath)
        {
            createDirectory(subpath);

            Image image;
            image = Bitmap.FromStream(file.InputStream);
            image.Save(Path.Combine(subpath, name));
            image.Dispose();
        }

        /// <summary>
        /// 新增縮圖資料夾
        /// </summary>
        /// <param name="minuploadPath"></param>
        private void createDirectory(string minuploadPath)
        {
            if (!System.IO.Directory.Exists(minuploadPath))
            {
                System.IO.Directory.CreateDirectory(Path.Combine(minuploadPath));
            }
        }
    }
}
