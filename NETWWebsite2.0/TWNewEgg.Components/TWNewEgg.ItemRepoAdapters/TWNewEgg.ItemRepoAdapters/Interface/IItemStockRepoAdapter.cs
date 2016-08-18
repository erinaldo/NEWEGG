using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IItemStockRepoAdapter
    {
        //int GetItemSellingQty(int itemID);

        /// <summary>
        /// 新增ItemStock
        /// </summary>
        /// <param name="argObjItemStock"></param>
        void Create(TWNewEgg.Models.DBModels.TWSQLDB.ItemStock argObjItemStock);

        /// <summary>
        /// 修改ItemStock
        /// </summary>
        /// <param name="argObjItemStock"></param>
        /// <returns></returns>
        bool Update(TWNewEgg.Models.DBModels.TWSQLDB.ItemStock argObjItemStock);
        ItemStock UpdateforModel(ItemStock ItemStock);
        /// <summary>
        /// 取得所有的ItemStock
        /// </summary>
        /// <returns></returns>
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> GetAllItemStock();

        /// <summary>
        /// 根據ItemStock ID取得物件
        /// </summary>
        /// <param name="argNumId">ItemStockId</param>
        /// <returns></returns>
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> GetItemStockById(int argNumId);

        /// <summary>
        /// 根據ItemStock 的Product ID取得物件
        /// </summary>
        /// <param name="argProductId">ItemStock.ProductID</param>
        /// <returns></returns>
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> GetItemStockByProductId(int argProductId);

        /// <summary>
        /// 根據ItemList取得SellingQty, 取得來源為程式的邏輯
        /// </summary>
        /// <param name="argListItemId">list of Item Id</param>
        /// <returns>Dictionary<int Item.ID, int SellingQty></returns>
        IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetSellingQtyByItemList(List<int> argListItemId);

        /// <summary>
        /// 取得所有的SellingQty, 資料龐大, 請勿直接使用
        /// </summary>
        /// <returns></returns>
        IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetAllViewQty();

        /// <summary>
        /// 根據CategoryId取得旗下所有的ItemStock的Qty, 取得來源為View_ItemSellingQty
        /// </summary>
        /// <param name="argNumCategoryId">CategoryId</param>
        /// <returns></returns>
        IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetItemSellingQtyByCategoryId(int argNumCategoryId);

        /// <summary>
        /// 根據ItemId取得SellingQty, 取得來源為View_ItemSellingQty
        /// </summary>
        /// <param name="argNumItemId">Item Id</param>
        /// <returns></returns>
        IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetItemSellingQtyByItemId(int argNumItemId);

        /// <summary>
        /// 根據ProductId取得SellingQty, 取得來源為View_ItemSellingQty
        /// </summary>
        /// <param name="argNumProductId">Product Id</param>
        /// <returns></returns>
        IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetItemSellingQtyByProductId(int argNumProductId);
        IQueryable<ItemStock> GetAll();
        List<ItemStock> UpdateAll(List<ItemStock> newDataList);
    }
}
