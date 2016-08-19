using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.PageMgmt
{
    public class Type1PMItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string DisplayPrice { get; set; }
        public string MarketPrice { get; set; }
        public string SubTitle { get; set; }
        public string ImageUrl { get; set; }
        public string LinkUrl { get; set; }
        public int SellingQty { get; set; }
        public int CategoryID { get; set; }
        public int StoreID { get; set; }
    }
}
