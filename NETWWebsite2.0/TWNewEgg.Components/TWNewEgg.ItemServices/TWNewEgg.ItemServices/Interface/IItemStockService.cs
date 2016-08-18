using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DBModels;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters;

namespace TWNewEgg.ItemServices.Interface
{
    public interface IItemStockService
    {
        /// <summary>
        /// 新增ItemStock
        /// </summary>
        /// <param name="argObjItemStock"></param>
        void Create(TWNewEgg.Models.DomainModels.Item.ItemStock argObjItemStock);

        /// <summary>
        /// 修改ItemStock
        /// </summary>
        /// <param name="argObjItemStock"></param>
        /// <returns></returns>
        bool Update(TWNewEgg.Models.DomainModels.Item.ItemStock argObjItemStock);

        /// <summary>
        /// 根據ID取得ItemStock
        /// </summary>
        /// <param name="argNumId">ItemStock ID</param>
        /// <returns>null or Object of ItemStock</returns>
        TWNewEgg.Models.DomainModels.Item.ItemStock GetItemStockById(int argNumId);

        /// <summary>
        /// 根據ProductId取得ItemStock
        /// </summary>
        /// <param name="argNumProductId">ItemStock.ProductID</param>
        /// <returns>null or ItemStock</returns>
        TWNewEgg.Models.DomainModels.Item.ItemStock GetItemStockByProductId(int argNumProductId);
        
        /// <summary>
        /// 取得所有Qty(請勿直接使用,會影響DB效能)
        /// </summary>
        /// <returns></returns>
        List<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty> GetAllViewQty();

        /// <summary>
        /// 根據CategoryId取得Qty
        /// </summary>
        /// <param name="argNumCategoryId">Category Id</param>
        /// <returns>list of View_ItemSellingQty</returns>
        List<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty> GetItemSellingQtyByCategoryId(int argNumCategoryId);

        /// <summary>
        /// 根據傳入的Item Id List取得SellingQty
        /// </summary>
        /// <param name="argListItemId">list of Item Id</param>
        /// <returns>Dictionary<ItemId, number of Qty></returns>
        Dictionary<int, int> GetSellingQtyByItemList(List<int> argListItemId);

        /// <summary>
        /// 根據Item ID取得Qty
        /// </summary>
        /// <param name="argNumItemId"></param>
        /// <returns></returns>
        TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty GetItemSellingQtyByItemId(int argNumItemId);

        /// <summary>
        /// 根據ProductID取得各個Qty
        /// </summary>
        /// <param name="argNumProductId">Product ID</param>
        /// <returns>list of View_ItemSellingQty</returns>
        List<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty> GetItemSellingQtyByProductId(int argNumProductId);
    }
}
