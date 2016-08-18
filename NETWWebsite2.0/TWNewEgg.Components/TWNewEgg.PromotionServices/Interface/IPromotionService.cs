using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.PromotionServices.Interface
{
    public interface IPromotionService
    {
        ResponseMessage<DbPromotionInfo> HasOverPurchaseDiscount(int itemID, string turnOn = "on");
    }
}
