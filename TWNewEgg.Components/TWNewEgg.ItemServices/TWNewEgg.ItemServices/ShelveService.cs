using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemServices
{
    public class ShelveService: IShelveService
    {
        private IItemRepoAdapter _ItemRepo = null;

        public ShelveService(IItemRepoAdapter argItemRepoAdapter)
        {
            this._ItemRepo = argItemRepoAdapter;
        }

        public void Shelve(int itemId)
        {
            Item item = this._ItemRepo.GetAll().Where(x=>x.ID == itemId).FirstOrDefault();
            if (item == null)
            {
                throw new Exception("商品不存在, itemId:" + itemId);
            }

            item.Status = (int)Item.status.已上架;
            _ItemRepo.UpdateItem(item);
        }

        public void Shelve(IEnumerable<int> itemIds)
        {
            throw new NotImplementedException();
        }

        public void ForceOffShelve(int itemId)
        {
            Item item = this._ItemRepo.GetIfAvailable(itemId);
            item.Status = (int)Item.status.強制下架;
            _ItemRepo.UpdateItem(item);
        }

        public void OffShelve(IEnumerable<int> itemIds)
        {
            throw new NotImplementedException();
        }
    }
}
