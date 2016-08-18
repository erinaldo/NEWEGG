using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.StorageServices.Model
{
    public class AzureBlobUpload
    {
        public string container { get; set; }
        public string blob { get; set; }
        public string absolutePath { get; set; }
    }
}
