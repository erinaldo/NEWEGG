using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters
{
    public class ItemStockRepoAdapter : IItemStockRepoAdapter
    {
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> _ItemSellingQtyRepo = null;
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> _ItemStockRepo = null;
        private IRepository<ItemStock> _itemStockRepo;
        public ItemStockRepoAdapter(
            IRepository<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> argItemSellingQtyRepo
            ,IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> argItemStockRepo
            , IRepository<ItemStock> itemStockRepo
            )
        {
            this._ItemSellingQtyRepo = argItemSellingQtyRepo;
            this._ItemStockRepo = argItemStockRepo; 
            this._itemStockRepo = itemStockRepo;
        }
        public IQueryable<ItemStock> GetAll()
        {
            return this._itemStockRepo.GetAll();
        }
        public ItemStock UpdateforModel(ItemStock ItemStock) 
        {
            this.Update(ItemStock);
            return ItemStock;
        }
        public List<ItemStock> UpdateAll(List<ItemStock> newDataList)
        {
            this._itemStockRepo.UpdateMany(newDataList);
            return newDataList;
        }
        public void Create(TWNewEgg.Models.DBModels.TWSQLDB.ItemStock argObjItemStock)
        {
            if (argObjItemStock == null)
            {
                return;
            }

            try
            {
                this._ItemStockRepo.Create(argObjItemStock);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public bool Update(TWNewEgg.Models.DBModels.TWSQLDB.ItemStock argObjItemStock)
        {
            if (argObjItemStock == null)
            {
                return false;
            }

            bool boolExec = false;
            try
            {
                this._ItemStockRepo.Update(argObjItemStock);
                boolExec = true;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }

        /// <summary>
        /// 取得所有的ItemStock
        /// </summary>
        /// <returns></returns>
        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> GetAllItemStock()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> queryResult = null;

            queryResult = this._ItemStockRepo.GetAll();

            return queryResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> GetItemStockById(int argNumId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> queryResult = null;

            queryResult = this.GetAllItemStock().Where(x => x.ID == argNumId);

            return queryResult;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> GetItemStockByProductId(int argProductId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock> queryResult = null;

            queryResult = this.GetAllItemStock().Where(x => x.ProductID == argProductId);

            return queryResult;
        }

        /*
        public int GetItemSellingQty(int itemID)
        {
            int qty = this._cartService.GetSellingQty(itemID, "Item");
            return qty;
        }*/

        /// <summary>
        /// 根據ItemList取得SellingQty
        /// </summary>
        /// <param name="argListItemId">list of Item Id</param>
        /// <returns></returns>
        public IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetSellingQtyByItemList(List<int> argListItemId)
        {
            if (argListItemId == null || argListItemId.Count <= 0)
            {
                return null;
            }

            IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> queryResult = null;

            queryResult = this.GetAllViewQty().Where(x => argListItemId.Contains(x.ID));

            return queryResult;
        }

        /// <summary>
        /// 取得所有的SellingQty, 資料龐大, 請勿直接使用
        /// </summary>
        /// <returns></returns>
        public IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetAllViewQty()
        {
            IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> queryResult = null;

            queryResult = this._ItemSellingQtyRepo.GetAll();

            return queryResult;
        }

        /// <summary>
        /// 根據CategoryId取得旗下所有的ItemStock的Qty, 取得來源為View_ItemSellingQty
        /// </summary>
        /// <param name="argNumCategoryId">CategoryId</param>
        /// <returns></returns>
        public IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetItemSellingQtyByCategoryId(int argNumCategoryId)
        {
            IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> queryResult = null;

            queryResult = this.GetAllViewQty().Where(x => x.CategoryID == argNumCategoryId);

            return queryResult;
        }

        /// <summary>
        /// 根據ItemId取得SellingQty, 取得來源為View_ItemSellingQty
        /// </summary>
        /// <param name="argNumItemId">Item Id</param>
        /// <returns></returns>
        public IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetItemSellingQtyByItemId(int argNumItemId)
        {
            IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> queryResult = null;

            queryResult = this.GetAllViewQty().Where(x => x.ID == argNumItemId);

            return queryResult;
        }

        /// <summary>
        /// 根據ProductId取得SellingQty, 取得來源為View_ItemSellingQty
        /// </summary>
        /// <param name="argNumProductId">Product Id</param>
        /// <returns></returns>
        public IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> GetItemSellingQtyByProductId(int argNumProductId)
        {
            IQueryable<Models.DBModels.TWSQLDB.View_ItemSellingQty> queryResult = null;

            queryResult = this.GetAllViewQty().Where(x => x.ProductId == argNumProductId);

            return queryResult;
        }
    }
}
