using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Product
{
    public class ProductDetailDM
    {
        public int ID { get; set; }
        public int FK { get; set; }
        public string SourceTable { get; set; }
        public string SellerProductID { get; set; }
        public string Name { get; set; }
        public string NameTW { get; set; }
        public string Description { get; set; }
        public string DescriptionTW { get; set; }
        public string SPEC { get; set; }
        public int ManufactureID { get; set; }
        public string Model { get; set; }
        public string BarCode { get; set; }
        public int SellerID { get; set; }

    }
}
