using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class ShoppingCart_View
    {
        public ShoppingCart_View()
        {
            this.ID = 0;
            this.Name = "無定義";
            this.Qty = 0;
            this.CartItemClassList = new List<CartItemClass_View>();
        }
        // CartID
        public int ID { get; set; }
        // CartQty
        public int Qty { get; set; }
        // CartName
        public string Name { get; set; }
        // 分類
        public List<CartItemClass_View> CartItemClassList { get; set; }
    }
}
