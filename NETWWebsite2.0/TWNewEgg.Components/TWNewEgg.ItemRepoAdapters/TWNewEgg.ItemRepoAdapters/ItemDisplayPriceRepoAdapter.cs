using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;

namespace TWNewEgg.ItemRepoAdapters
{
    public class ItemDisplayPriceRepoAdapter : IItemDisplayPriceRepoAdapter
    {
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> _itemDisplayPriceRepo;
        public ItemDisplayPriceRepoAdapter(IRepository<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> itemDisplayPriceRepo)
        {
            this._itemDisplayPriceRepo = itemDisplayPriceRepo;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> GetAll()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> ItemDisplayPriceList = this._itemDisplayPriceRepo.GetAll();
            return ItemDisplayPriceList;
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> GetItemDisplayPriceList(List<int> itemIDList)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> ItemDisplayPriceList = this._itemDisplayPriceRepo.GetAll().Where(x => itemIDList.Contains(x.ItemID));
            return ItemDisplayPriceList;
        }
    }
}
