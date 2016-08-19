using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Redeem
{
    public class PromotionInput
    {
        public int ItemID { get; set; }
        public int Price { get; set; }
        public int Qty { get; set; }
        public decimal SumPrice { get; set; }
    }
}
