using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class CartItemClass
    {
        public CartItemClass()
        {
            this.ID = 0;
            this.Title = "";
            this.CartItemList = new List<CartItem>();
            this.IsCheckout = true;
        }
        public int ID { get; set; }
        public int TypeID { get; set; }
        public int TypeQty { get; set; }   
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> StoreID { get; set; }
        public string Title { get; set; }
        public Nullable<int> SellerID { get; set; }
        public Nullable<decimal> FreeCost { get; set; }
        // ConstShowAll
        public Nullable<int> ShowAll { get; set; }
        public Nullable<int> Showorder { get; set; }
        public bool IsCheckout { get; set; }
        public List<CartItem> CartItemList { get; set; }
    }
}
