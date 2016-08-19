using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class CartStep1Data
    {
        public CartStep1Data()
        {
            this.CartItemDetailList_View = new List<CartItemDetail_View>();
            this.CartPaytype_View = new CartPaytype_View();
            this.CouponList = new List<Redeem.Coupon>();
            this.AmountsPayable = 0;
        }

        public string SerialNumber { get; set; }
        public List<CartItemDetail_View> CartItemDetailList_View { get; set; }
        public CartPaytype_View CartPaytype_View { get; set; }
        public int PromotionPriceSum { get; set; }
        public int CartTypeID { get; set; }
        public List<TWNewEgg.Models.ViewModels.Redeem.Coupon> CouponList { get; set; }
        public int AmountsPayable { get; set; }

        /// <summary>
        /// 同意癈四機回收
        /// Y=同意, 預設NULL
        /// </summary>
        public string AgreedDiscard4 { get; set; }

    }

    public class CartItemDetail_View
    {
        public int ItemID { get; set; }
        public int Price { get; set; }
        public int Category { get; set; }
        public int Qty { get; set; }
    }

    public class CartPaytype_View
    {
        public int PayTypeGroupID { get; set; }
        public int PayType0rateNum { get; set; }
        public int BankID { get; set; }
    }
}
