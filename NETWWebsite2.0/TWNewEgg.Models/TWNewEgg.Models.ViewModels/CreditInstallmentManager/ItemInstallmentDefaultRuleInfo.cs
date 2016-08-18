using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.CreditInstallmentManager
{
    public class ItemInstallmentDefaultRuleInfo
    {
        public int ID { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public int Price { get; set; }
        
        /// <summary>
        /// 分期期數
        /// </summary>
        public int Installment { get; set; }

        /// <summary>
        /// 毛利(商品毛利 - 分期利率)
        /// </summary>
        public decimal Rate { get; set; }
    }
}
