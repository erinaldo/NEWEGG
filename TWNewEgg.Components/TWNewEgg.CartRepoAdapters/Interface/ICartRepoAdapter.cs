using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface ICartRepoAdapter
    {
        int GetGroupBuyNumber(int ItemID, DateTime BeginDate, DateTime EndDate);
        Cart GetCart(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM);
        Cart GetCart(string ID);
        Cart Update(Cart cart);
        IQueryable<Process> GetProcessByCartID(string CartId);
    }
}
