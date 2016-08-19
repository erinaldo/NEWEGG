using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class SOInfo
    {
        public enum nPayType
        {
            信用卡一次付清 = 1,
            三期零利率 = 3,
            六期零利率 = 6,
            九期零利率 = 9,
            十期零利率 = 10,
            十二期零利率 = 12,
            十八期零利率 = 18,
            二十四期零利率 = 24,
            三期分期 = 103,
            六期分期 = 106,
            九期分期 = 109,
            十期分期 = 110,
            十二期分期 = 112,
            十八期分期 = 118,
            二十四期分期 = 124,
            三十期分期 = 130,
            信用卡紅利折抵 = 201,
            網路ATM = 30,
            貨到付款 = 31,
            超商付款 = 32,
            電匯 = 33,
            實體ATM = 34,
            歐付寶儲值支付 = 501
        }

        public SOBase Main { get; set; }
        public List<SOItemBase> SOItems { get; set; }
    }
}
