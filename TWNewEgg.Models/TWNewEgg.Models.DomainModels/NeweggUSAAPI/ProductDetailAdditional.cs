using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.NeweggUSAAPI
{

    public class ProductDetailAdditional
    {
        public bool IsShellShockerItem { get; set; }
        public Category Category { get; set; }
        public string Model { get; set; }
        public string StockDescription { get; set; }
        public string PromotionCode { get; set; }
    }
    public class Category
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }
}
