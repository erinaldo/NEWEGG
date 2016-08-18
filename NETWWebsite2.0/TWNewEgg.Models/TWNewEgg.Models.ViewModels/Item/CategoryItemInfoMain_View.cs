using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Item
{
    public class CategoryItemInfoMain_View
    {
        public CategoryItemInfoMain_View() {
            this.CategoryItemInfo_ViewList = new List<CategoryItemInfo_View>();
            this.PriceWithQty_ViewList = new List<TWNewEgg.Models.ViewModels.Property.PriceWithQty_View>();
            this.ShowPageList = new List<TWNewEgg.Models.ViewModels.Page.ShowPage>();
        }
        public int ID { get; set; }
        public int? Page { get; set; }
        public int? TotalPage { get; set; }
        public string OrderBy { get; set; }
        public List<CategoryItemInfo_View> CategoryItemInfo_ViewList { get; set; }
        public List<TWNewEgg.Models.ViewModels.Property.PriceWithQty_View> PriceWithQty_ViewList { get; set; }
        // 畫面分頁計算
        public List<TWNewEgg.Models.ViewModels.Page.ShowPage> ShowPageList { get; set; }
    }
}
