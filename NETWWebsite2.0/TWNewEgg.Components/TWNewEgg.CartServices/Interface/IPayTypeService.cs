using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.CartServices.Interface
{
    public interface IPayTypeService
    {
        /// <summary>
        /// 根據SalesOrder的Paytype欄位，找最第一順位的PayType回傳
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Models.DBModels.TWSQLDB.PayType GetPayType(SOBase order);

    }
}
