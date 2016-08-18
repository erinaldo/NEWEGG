using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class CartItemGroup
    {
        public CartItemGroup(){
            this.CartItemList = new List<CartItem>();
        }
        public List<CartItem> CartItemList { get; set; }
        public string Status { get; set; }
        public int GroupID { get; set; }
        public int TypeID { get; set; }
        public int TypeQty { get; set; }   
    }
}
