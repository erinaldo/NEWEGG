using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.StorageServices.Interface
{
    public interface IAure
    {
        List<string> changeFileContentTypeFileByBatch(string containerName, string direction, string contentType = "image/jpeg");
        void ChangeFileContentTypeForBlob(string containerName, string contentType, string blob);
    }
}
