using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DBModels;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemRepoAdapters;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.ItemServices
{
    public class ItemStockService:IItemStockService
    {
        private IItemStockRepoAdapter _ItemStockRepo = null;

        public ItemStockService(IItemStockRepoAdapter argItemStockRepo)
        {
            this._ItemStockRepo = argItemStockRepo;
        }


        public void Create(ItemStock argObjItemStock)
        {
            if (argObjItemStock == null)
            {
                return;
            }

            TWNewEgg.Models.DBModels.TWSQLDB.ItemStock objItemStock = null;
            objItemStock = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock>(argObjItemStock);

            try
            {
                this._ItemStockRepo.Create(objItemStock);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        public bool Update(ItemStock argObjItemStock)
        {
            if (argObjItemStock == null)
            {
                return false;
            }

            TWNewEgg.Models.DBModels.TWSQLDB.ItemStock objItemStock = null;
            bool boolExec = false;
            objItemStock = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.ItemStock>(argObjItemStock);

            try
            {
                boolExec = this._ItemStockRepo.Update(objItemStock);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }

        public ItemStock GetItemStockById(int argNumId)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.ItemStock objDbItemStock = null;
            TWNewEgg.Models.DomainModels.Item.ItemStock objResult = null;

            try
            {
                objDbItemStock = this._ItemStockRepo.GetItemStockById(argNumId).FirstOrDefault();
                if (objDbItemStock != null)
                {
                    objResult = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.ItemStock>(objDbItemStock);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return objResult;
        }

        public ItemStock GetItemStockByProductId(int argNumProductId)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.ItemStock objDbItemStock = null;
            TWNewEgg.Models.DomainModels.Item.ItemStock objResult = null;

            try
            {
                objDbItemStock = this._ItemStockRepo.GetItemStockByProductId(argNumProductId).FirstOrDefault();
                if (objDbItemStock != null)
                {
                    objResult = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.ItemStock>(objDbItemStock);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return objResult;
        }

        public List<View_ItemSellingQty> GetAllViewQty()
        {
            List<View_ItemSellingQty> listResult = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> listDbSearch = null;

            try
            {
                listDbSearch = this._ItemStockRepo.GetAllViewQty().ToList();
                if (listDbSearch != null && listDbSearch.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty>>(listDbSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return listResult;
        }

        public List<View_ItemSellingQty> GetItemSellingQtyByCategoryId(int argNumCategoryId)
        {
            List<View_ItemSellingQty> listResult = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> listDbSearch = null;

            try
            {
                listDbSearch = this._ItemStockRepo.GetItemSellingQtyByCategoryId(argNumCategoryId).ToList();
                if (listDbSearch != null && listDbSearch.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty>>(listDbSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return listResult;
        }

        public Dictionary<int, int> GetSellingQtyByItemList(List<int> argListItemId)
        {
            if (argListItemId == null || argListItemId.Count <= 0)
            {
                return null;
            }

            List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> listDbSearch = null;
            TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objTempSellingQty = null;
            Dictionary<int, int> dictResult = null;

            try
            {
                //設定SellingQty初始值
                dictResult = argListItemId.Select(x => new { Key = x, Value = 0 }).ToDictionary(x => x.Key, x => x.Value);

                //查詢SellingQty
                listDbSearch = this._ItemStockRepo.GetSellingQtyByItemList(argListItemId).ToList();
                if (listDbSearch != null && listDbSearch.Count > 0)
                {
                    foreach (TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objSellingQty in listDbSearch)
                    {
                        dictResult[objSellingQty.ID] = objSellingQty.SellingQty ?? 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return dictResult;
        }

        public View_ItemSellingQty GetItemSellingQtyByItemId(int argNumItemId)
        {
            View_ItemSellingQty objResult = null;
            TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objDbSearch = null;

            try
            {
                objDbSearch = this._ItemStockRepo.GetItemSellingQtyByItemId(argNumItemId).FirstOrDefault();
                if(objDbSearch != null)
                {
                    objResult = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty>(objDbSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return objResult;
        }

        public List<View_ItemSellingQty> GetItemSellingQtyByProductId(int argNumProductId)
        {
            List<View_ItemSellingQty> listResult = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> listDbSearch = null;

            try
            {
                listDbSearch = this._ItemStockRepo.GetItemSellingQtyByProductId(argNumProductId).ToList();
                if (listDbSearch != null && listDbSearch.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty>>(listDbSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return listResult;
        }
    }
}
