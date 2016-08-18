using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TWNewEgg.Models.ViewModels.FileUpload
{
    public class UploadModel
    {
        public int? chunk { get; set; }
        public string name { get; set; }
        public string subpath { get; set; }
        public string minSubpath { get; set; }
        public HttpPostedFileBase file { get; set; }
        public int resizeSet { get; set; }
        public float resizeValue { get; set; }//決定縮圖數字或比例 EX:以寬為100等比例縮圖=>resizeValue=100，以80%等比例縮圖=>resizeValue=0.8
        public int validateScale { get; set; }
        public int validateWidth { get; set; }
        public int validateHeight { get; set; }
        public string message { get; set; }
        public string imageType { get; set; }
    }
}
