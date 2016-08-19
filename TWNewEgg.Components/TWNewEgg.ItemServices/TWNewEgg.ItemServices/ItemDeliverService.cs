using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemdeliverblackAdapters.Interface;
using TWNewEgg.ItemDeliverWhiteRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;

namespace TWNewEgg.ItemServices
{
    public class ItemDeliverService : IItemDeliver
    {
        private IItemDeliverWhiteRepoAdapters _iItemDeliverWhiteRepoAdapters;
        private IItemdeliverblackRepoAdapters _iItemdeliverblackRepoAdapters;

        public ItemDeliverService(IItemDeliverWhiteRepoAdapters iItemDeliverWhiteRepoAdapters, IItemdeliverblackRepoAdapters iItemdeliverblackRepoAdapters)
        {
            this._iItemDeliverWhiteRepoAdapters = iItemDeliverWhiteRepoAdapters;
            this._iItemdeliverblackRepoAdapters = iItemdeliverblackRepoAdapters;
        }
        public TWNewEgg.Models.DomainModels.Item.ItemDeliver getItemBalckAndWhite(List<int>itemid, List<int> payTypeid)
        {
            TWNewEgg.Models.DomainModels.Item.ItemDeliver itemResult = new Models.DomainModels.Item.ItemDeliver();
            itemResult.itemBlack = this.getItemDeliverBlack(itemid, payTypeid);
            itemResult.itemWhite = this.getItemDeliverWhite(itemid, payTypeid);
            return itemResult;
        }
        private List<TWNewEgg.Models.DBModels.TWSQLDB.ItemDeliverWhite> getItemDeliverWhite(List<int> itemid, List<int> paytypeid)
        {
            List<TWNewEgg.Models.DBModels.TWSQLDB.ItemDeliverWhite> whiteResult = new List<Models.DBModels.TWSQLDB.ItemDeliverWhite>();
            whiteResult = this._iItemDeliverWhiteRepoAdapters.GetAll().Where(p => itemid.Contains(p.ItemID) && paytypeid.Contains(p.PayTypeID)).ToList();
            return whiteResult;
        }
        private List<TWNewEgg.Models.DBModels.TWSQLDB.ItemDeliverBlack> getItemDeliverBlack(List<int> itemid, List<int> paytypeid)
        {
            List<TWNewEgg.Models.DBModels.TWSQLDB.ItemDeliverBlack> blackResult = new List<Models.DBModels.TWSQLDB.ItemDeliverBlack>();
            blackResult = this._iItemdeliverblackRepoAdapters.GetAll().Where(p => itemid.Contains(p.ItemID) && paytypeid.Contains(p.PayTypeID)).ToList();
            return blackResult;
        }
       
    }
}
