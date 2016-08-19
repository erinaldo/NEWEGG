using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.ItemInstallment
{
    public class DefaultInstallment
    {
        public int ID { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 信用卡分期
        /// </summary>
        public int Installment { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        /// <remarks>商品毛利率-分期利率</remarks>
        public decimal Rate { get; set; }
    }
}
