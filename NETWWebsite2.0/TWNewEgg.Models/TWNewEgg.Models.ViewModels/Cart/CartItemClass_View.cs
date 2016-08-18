using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class CartItemClass_View
    {
        public CartItemClass_View()
        {
            this.ID = 0;
            this.Title = "";
            this.CartItemList = new List<CartItem_View>();
            this.AdditionalItemList = new List<AdditionalItem_View>();
        }
        public int ID { get; set; }
        public int TypeID { get; set; }
        public int TypeQty { get; set; }   
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> StoreID { get; set; }
        public string Title { get; set; }
        public string ItemIDListString { get; set; }
        public Nullable<int> SellerID { get; set; }
        public Nullable<decimal> FreeCost { get; set; }
        // ConstShowAll
        public Nullable<int> ShowAll { get; set; }
        public Nullable<int> Showorder { get; set; }
        public bool IsCheckout { get; set; }
        public List<CartItem_View> CartItemList { get; set; }
        public List<AdditionalItem_View> AdditionalItemList { get; set; }

    }
}
