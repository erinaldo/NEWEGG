using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.GroupBuyRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using LinqKit;

namespace TWNewEgg.GroupBuyRepoAdapters
{
    public class GroupBuyRepoAdapter : IGroupBuyRepoAdapter
    {      
        private IRepository<GroupBuy> _groupBuy;

        public GroupBuyRepoAdapter(IRepository<GroupBuy> groupBuy)
        {
            this._groupBuy = groupBuy;
        }

        public GroupBuy GetGroupBuyItem(int GroupID, int ItemID)
        {
            GroupBuy GroupBuyItem = new GroupBuy();
            var query = PredicateBuilder.True<GroupBuy>();
            if (GroupID > 0) { 
                query = query.And(f=>f.ID == GroupID);
            } 

            query = query.And(f=>f.ItemID == ItemID);
            GroupBuyItem = this._groupBuy.GetAll().Where(query).AsExpandable().FirstOrDefault();
            return GroupBuyItem;
        }
    }
}
