using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.MobileStoreRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.MobileStoreRepoAdapters
{
    public class MobileStoreRepoAdapter : IMobileStoreRepoAdapter
    {
        private IRepository<ItemForChoice> _itemForChoiceRepo;
        private IRepository<GroupBuy> _groupBuy;

        public MobileStoreRepoAdapter(IRepository<ItemForChoice> itemForChoiceRepo, IRepository<GroupBuy> groupBuy)
        {
            this._itemForChoiceRepo = itemForChoiceRepo;
            this._groupBuy = groupBuy;
        }
        public IQueryable<ItemForChoice> ItemForChoice_GetAll()
        {
            return this._itemForChoiceRepo.GetAll();
        }
        public IQueryable<GroupBuy> GroupBuy_GetAll()
        {
            return this._groupBuy.GetAll();
        }
    }
}
