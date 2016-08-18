using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.StorageServices.Interface
{
    /// <summary>
    /// CDN功能介面
    /// </summary>
    public interface ICDNAdapter
    {
        /// <summary>
        /// 刪除CDN上的Content
        /// </summary>
        /// <param name="purgeFiles">URI</param>
        void Purge(List<string> purgeFiles);
    }
}
