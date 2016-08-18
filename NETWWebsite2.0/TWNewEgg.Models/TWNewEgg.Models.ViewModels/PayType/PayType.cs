using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.PayType
{
    public class PayType
    {
        public class ItemPayType
        {
            public int PayType0rateNum { get; set; }
            //顯示在前台的可用銀行
            public string PayTypeBankStrForList { get; set; }
            //期數
            public int Staging { get; set; }
            //利息
            public string InsRate { get; set; }
            //可使用的銀行數量
            public int canUseBankCount { get; set; }
        }

    }
}
