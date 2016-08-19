using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.CartPayment
{
    public class PaymentTermID
    {
        public const string 信用卡一次付清 = "0101";
        public const string 信用卡分期 = "0102";
        
        public const string 信用卡紅利折抵 = "0103";
        public const string 實體ATM = "0201";
        public const string 網路ATM ="0202";
        public const string 貨到付款 ="0301";
        public const string 超商付款 ="0401";
        public const string 電匯 = "9901";
        public const string 儲值支付 = "9902";
    }
}
