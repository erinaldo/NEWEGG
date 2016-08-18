using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters
{
    public class ItemRepoAdapter : IItemRepoAdapter
    {
        private IRepository<Item> _itemRepo;
        private IRepository<ItemCategory> _itemCategoryRepo;

        public IQueryable<Item> GetAll()
        {
            return this._itemRepo.GetAll();
        }

        public Item UpdateItem(Item newData)
        {
            _itemRepo.Update(newData);
            return newData;
        }

        public List<Item> UpdateItemList(List<Item> newDataList)
        {

            this._itemRepo.UpdateMany(newDataList);
            return newDataList;
        }

        public ItemRepoAdapter(IRepository<Item> itemRepo, IRepository<ItemCategory> itemCategoryRepo)
        {
            this._itemRepo = itemRepo;
            this._itemCategoryRepo = itemCategoryRepo;
        }
        
        public IQueryable<Item> GetAvailableAndVisible(int categoryId)
        {
            var result = this._itemRepo
                .GetAll()
                .Where(x => x.CategoryID == categoryId && x.Status == 0 && x.ShowOrder >= 0);
            return result;
        }

        public IQueryable<Item> GetCrossCategoryAvailableAndVisible(int categoryId)
        {
            var result = this._itemCategoryRepo.GetAll()
                .Where(x => x.CategoryID == categoryId)
                .Join(this._itemRepo.GetAll().Where(x => x.Status == 0 && x.ShowOrder >= 0),
                    ic => ic.ItemID,
                    i => i.ID, (ic, i) => i);
            return result;
        }

        public Item GetIfAvailable(int itemId)
        {
            var result = this._itemRepo.Get(x => x.ID == itemId && x.Status == 0);
            return result;
        }

        public IQueryable<Item> GetAvailableAndVisibleItemList(List<int> itemIDList)
        {
            var result = this._itemRepo.GetAll().Where(x => itemIDList.Contains(x.ID) && x.Status == 0 && x.ShowOrder >= 0);
            return result;
        }

        public Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item> GetItemList(List<int> ItemListID)
        {
            var result = this._itemRepo.GetAll().Where(x => ItemListID.Contains(x.ID));
            Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item> returnItem = new Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.Item>();

            foreach (var item in result)
            {
                returnItem.Add(item.ID, item);
            }

            return returnItem;
        }

        /// <summary>
        /// 依多筆賣場編號查詢多筆交易模式(Group by 不重複)
        /// </summary>
        /// <param name="listItem">多筆賣場編號</param>
        /// <returns>多筆交易模式</returns>
        public List<int> GetListGroupByItemDelvType(List<int> listItem)
        {
            List<int> listDelvType = new List<int>();
            int delvType;

            if (listItem == null)
            {
                return listDelvType;
            }

            //各個賣場編號查詢其交易模式
            foreach (int item in listItem)
            {
                delvType = this._itemRepo.GetAll().Where(d => d.ID == item).Select(d => d.DelvType).FirstOrDefault();

                listDelvType.Add(delvType);
            }

            //多筆交易模式Group by
            listDelvType = listDelvType.GroupBy(d => d).Select(d => d.Key).ToList();


            return listDelvType;
        }
    }
}
