using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.ViewModels;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class ItemGroup_View
    {
        public ItemGroup_View()
        {
            this.CartItemList = new List<CartItem_View>();
            this.CartItemClass_ViewList = new List<CartItemClass_View>();
            this.CartPayTypeGroupList = new List<CartPayTypeGroup_View>();
            this.PromotionInputList = new List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View>();
            this.PromotionItemIDList = new List<int>();
            this.ShowPageList = new List<TWNewEgg.Models.ViewModels.Page.ShowPage>();
            this.TypeName = "Null";
            this.ViewPage = 1;
        }
        public List<CartItem_View> CartItemList { get; set; }
        public List<CartItemClass_View> CartItemClass_ViewList { get; set; }
        public List<CartPayTypeGroup_View> CartPayTypeGroupList { get; set; }
        public List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View> PromotionInputList { get; set; }
        public List<int> PromotionItemIDList { get; set; }
        // 購物車顯示排列方式
        public int GroupID { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public string OrderBy { get; set; }
        public int TypeQty { get; set; }
        public int DiscountSum { get; set; }
        public int ShipDiscountSum { get; set; }
        // 頁面顯示頁面
        public int ViewPage { get; set; }
        // 總共頁面
        public int TotalPage { get; set; }
        // 畫面分頁計算
        public List<TWNewEgg.Models.ViewModels.Page.ShowPage> ShowPageList { get; set; }
    }
}
