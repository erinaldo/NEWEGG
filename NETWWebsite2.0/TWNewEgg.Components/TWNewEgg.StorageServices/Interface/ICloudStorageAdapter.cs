using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.StorageServices.Interface
{
    public interface ICloudStorageAdapter
    {
        void Upload(string path);
        void DeleteLocal(string path);
    }
}
