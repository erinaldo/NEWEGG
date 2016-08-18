using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class GetPromotionDetail_View
    {
        public GetPromotionDetail_View()
        {
            this.ItemIDList = new List<int>();
            this.Price = 0;
        }

        public List<int> ItemIDList { get; set; }
        public int Price { get; set; }
    }
}
