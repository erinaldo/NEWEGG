using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("product")]
    public class Product
    {
        //狀態
        public enum status
        {
            正常 = 0
        }
        //交易模式
        public enum DelvTypes
        {
            切貨 = 0,
            間配 = 1,
            直配 = 2,
            三角 = 3,
            國外直配 = 4,
            自貿區 = 5,
            海外切貨 = 6,
            B2C直配 = 7,
            MKPL寄倉 = 8,
            B2C寄倉 = 9
        }
        public Product()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            this.CreateDate = defaultDate;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        //2013.11.1 add columns by Ice begin      
        public string Note { get; set; }
        public string IsMarket { get; set; }
        public string Is18 { get; set; }
        public string IsShipDanger { get; set; }
        public string IsChokingDanger { get; set; }
        public string UPC { get; set; }                     //2013.11.27 add column to product by Ice
        public string MenufacturePartNum { get; set; }      //2013.11.27 add column to product by Ice
        //2013.11.1 add columns by Ice end
        public Nullable<decimal> SupplyShippingCharge { get; set; }
    }
}
