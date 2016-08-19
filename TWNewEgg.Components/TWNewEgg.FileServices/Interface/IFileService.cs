using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TWNewEgg.FileServices.Interface
{
    public interface IFileService
    {
        void fileUpload(HttpPostedFileBase file, int? chunk, string name, string uploadPath);
    }
}
