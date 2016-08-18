using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.ItemRepoAdapters;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.SellerRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.Seller;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.StoreRepoAdapters.Interface;

namespace TWNewEgg.ItemServices
{
    public class ItemInfoService :IItemInfoService
    {
        private IDbItemInfoRepoAdapter _dbItemInfoRepoAdapter;
        private IStoreRepoAdapter _iStoreRepoAdapter;
        private IItemRepoAdapter _ItemRepo = null;

        public ItemInfoService(IDbItemInfoRepoAdapter dbItemInfoRepoAdapter, IStoreRepoAdapter iStoreRepoAdapter, IItemRepoAdapter argItemRepoAdapter)
        {
            this._dbItemInfoRepoAdapter = dbItemInfoRepoAdapter;
            this._iStoreRepoAdapter = iStoreRepoAdapter;
            this._ItemRepo = argItemRepoAdapter;
        }

        public ItemInfo GetItemInfo(int itemId)
        {
            IQueryable<DbItemInfo> query = this._dbItemInfoRepoAdapter.GetDbItemInfos();
            DbItemInfo joinedItemData = query.Where(x => x.item.ID == itemId).FirstOrDefault();
            
            if (joinedItemData == null)
            {
                return null;
            }
            
            ItemInfo itemInfo = new ItemInfo()
            {
                ItemBase = ModelConverter.ConvertTo<ItemBase>(joinedItemData.item),
                ProductBase = ModelConverter.ConvertTo<ProductBase>(joinedItemData.product),
                SellerBase = ModelConverter.ConvertTo<SellerBase>(joinedItemData.seller),
                IsChooseAny = IsChooseAny(joinedItemData.item.CategoryID) ? 1 : 0
            };

            return itemInfo;
        }

        public Dictionary<int, ItemInfo> GetItemInfoList(List<int> itemId)
        {
            IQueryable<DbItemInfo> query = this._dbItemInfoRepoAdapter.GetDbItemInfos();
            Dictionary<int, ItemInfo> itemInfoList = new Dictionary<int, ItemInfo>();
            List<DbItemInfo> joinedItemData = query.Where(x => itemId.Contains(x.item.ID)).ToList();

            if (joinedItemData == null)
            {
                throw new NullReferenceException("can't find ItemInfo");
            }

            foreach(var temp in joinedItemData)
            {
                ItemInfo itemInfo = new ItemInfo()
                {
                    ItemBase = ModelConverter.ConvertTo<ItemBase>(temp.item),
                    ProductBase = ModelConverter.ConvertTo<ProductBase>(temp.product),
                    SellerBase = ModelConverter.ConvertTo<SellerBase>(temp.seller),
                    IsChooseAny = IsChooseAny(temp.item.CategoryID) ? 1 : 0
                };
                itemInfoList.Add(itemInfo.ItemBase.ID, itemInfo);
            };
            return itemInfoList;
        }

        public bool IsChooseAny(int categoryID)
        {
            bool isOptionStore = false;

            DateTime datetimeNow = DateTime.UtcNow.AddHours(8);

            SubCategory_OptionStore optionStore = this._iStoreRepoAdapter.OptionStore_GetAll().Where(x => x.CategoryID == categoryID && x.ShowAll == (int)TWNewEgg.Models.DBModels.TWSQLDB.SubCategory_OptionStore.ConstShowAll.Show && (x.DateEnd > datetimeNow || x.DateEnd == null) && (x.DateStart < datetimeNow || x.DateStart == null)).OrderBy(x => x.Showorder).FirstOrDefault();

            if (optionStore != null)
            {
                isOptionStore = true;
            }

            return isOptionStore;
        }

        public List<ItemBase> GetAvailableAndVisible(int argNumCategoryId)
        {
            List<Item> listDbItem = null;
            List<ItemBase> listItem = null;
            IQueryable<Item> queryItemSearch = this._ItemRepo.GetAvailableAndVisible(argNumCategoryId);
            listDbItem = queryItemSearch.ToList();

            if (listDbItem != null && listDbItem.Count > 0)
            {
                listItem = ModelConverter.ConvertTo<List<ItemBase>>(listDbItem);
            }

            return listItem;
        }

    }
}
