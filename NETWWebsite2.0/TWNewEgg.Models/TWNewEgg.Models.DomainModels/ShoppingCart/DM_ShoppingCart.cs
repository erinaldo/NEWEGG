using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.ShoppingCartPayType
{
    public class DM_ShoppingCart
    {
        public enum CartType
        {
            全部 = -1,
            新蛋購物車 = 1,
            海外購物車 = 2,
            任選館購物車 = 3,
            追蹤清單購物車 = 4
        };

        public int CarType { get; set; }
        public List<int> Items { get; set; }
    }
}
