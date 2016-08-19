using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class PostCartItem
    {
        public int ItemID { get; set; }
        public int Qty { get; set; }
        public List<PostCartItem> PostCartItemList { get; set; }
    }
}
