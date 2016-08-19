using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class PostCartItemGroup_View
    {
        public List<PostCartItem_View> PostCartItem { get; set; }
        public string CartType { get; set; }
    }
}
