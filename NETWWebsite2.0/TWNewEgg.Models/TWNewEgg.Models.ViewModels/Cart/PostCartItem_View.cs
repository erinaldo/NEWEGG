using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class PostCartItem_View
    {
        public int ItemID { get; set; }
        public int Qty { get; set; }
        public List<PostCartItem_View> PostCartItemList { get; set; }
    }
}
