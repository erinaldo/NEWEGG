using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ItemServices.Interface
{
    public interface IShelveService
    {
        /// <summary>
        /// 上架單一商品
        /// </summary>
        /// <param name="itemId"></param>
        void Shelve(int itemId);

        /// <summary>
        /// 上架多個商品
        /// </summary>
        /// <param name="itemIds"></param>
        void Shelve(IEnumerable<int> itemIds);

        /// <summary>
        /// 強制下架單一商品
        /// </summary>
        /// <param name="itemId"></param>
        void ForceOffShelve(int itemId);

        /// <summary>
        /// 下嫁多個商品
        /// </summary>
        /// <param name="itemIds"></param>
        void OffShelve(IEnumerable<int> itemIds);
    }
}
