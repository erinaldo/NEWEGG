using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ProductBase
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
        public Nullable<int> DelvType { get; set; }
        public Nullable<int> PicStart { get; set; }
        public Nullable<int> PicEnd { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public int Status { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<int> SaleType { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> TradeTax { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public int? Warranty { get; set; }
        public string Note { get; set; }
        public string IsMarket { get; set; }
        public string Is18 { get; set; }
        public string IsShipDanger { get; set; }
        public string IsChokingDanger { get; set; }
        public string UPC { get; set; }                     //2013.11.27 add column to product by Ice
        public string MenufacturePartNum { get; set; }      //2013.11.27 add column to product by Ice
        public Nullable<decimal> SupplyShippingCharge { get; set; }
    }
}
