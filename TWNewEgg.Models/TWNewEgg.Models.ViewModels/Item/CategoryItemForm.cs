using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Item
{
    public class CategoryItemForm
    {
        public CategoryItemForm() {
            this.IsFirstTime = 1;
        }

        public int CategoryID { get; set; }
        public string FilterID { get; set; }
        public int? minPrice { get; set; }
        public int? maxPrice { get; set; }
        public int? CountryID { get; set; }
        public int? BrandID { get; set; }
        public int? length { get; set; }
        public int start { get; set; }
        public int orderBy { get; set; }
        public int? OnePageItemsQty { get; set; }
        public int? Qty { get; set; }
        public int? NowPage { get; set; }
        public int? TotalPage { get; set; }
        // 0:第一次
        public int? IsFirstTime { get; set; }
    }
}
