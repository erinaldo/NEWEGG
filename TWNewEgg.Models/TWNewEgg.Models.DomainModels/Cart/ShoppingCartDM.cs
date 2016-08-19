using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class ShoppingCartDM
    {
        public enum CartType
        {
            全部 = -1,
            新蛋購物車 = 1,
            海外購物車 = 2,
            任選館購物車 = 3,
            追蹤清單購物車 = 4
        };

        public ShoppingCartDM()
        {
            this.ID = 0;
            this.Name = "無定義";
            this.Qty = 0;
            this.CartItemClassList = new List<CartItemClass>();
        }
        // CartID
        public int ID { get; set; }
        // CartQty
        public int Qty { get; set; }
        // CartName
        public string Name { get; set; }
        // 分類
        public List<CartItemClass> CartItemClassList { get; set; }
    }
}
