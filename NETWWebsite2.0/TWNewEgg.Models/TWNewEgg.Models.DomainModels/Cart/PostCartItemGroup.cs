using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class PostCartItemGroup
    {
        public List<PostCartItem> PostCartItem { get; set; }
        public string CartType { get; set; }
    }
}
