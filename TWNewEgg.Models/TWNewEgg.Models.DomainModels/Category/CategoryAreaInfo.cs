using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Property;
using TWNewEgg.Models.DomainModels.Seller;

namespace TWNewEgg.Models.DomainModels.Category
{
    public class CategoryAreaInfo
    {
        public CategoryAreaInfo() {
            this.ItemBaseList = new List<ItemBase>();
            this.PriceWithQtyList = new List<PriceWithQty>();
        }

        public List<ItemBase> ItemBaseList { get; set; }
        public List<PriceWithQty> PriceWithQtyList { get; set; }
        public int Qty { get; set; }
        public int NowPage { get; set; }
        public int TotalPage { get; set; }       
    }
}
