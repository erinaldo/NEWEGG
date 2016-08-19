using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public enum CartPayTypeGroupenum
    {
        信用卡 = 1,
        信用卡紅利折抵 = 2,
        貨到付款 = 3,
        超商付款 = 4,
        實體ATM = 5,
        網路ATM = 6,
        儲值支付 = 7,
        電匯 = 8
    }

    public class CartPayTypeGroup_View
    {
        public List<CartPayType_View> CartPayType_View { get; set; }
        public int PayTypeGroupID { get; set; }
        public string PayTypeGroupName { get; set; }
    }
}
