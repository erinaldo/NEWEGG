using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.GroupBuyRepoAdapters.Interface
{
    public interface IGroupBuyRepoAdapter
    {
        GroupBuy GetGroupBuyItem(int GroupID, int ItemID);
    }
}
