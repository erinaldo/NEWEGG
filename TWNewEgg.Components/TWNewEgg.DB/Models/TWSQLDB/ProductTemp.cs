using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("producttemp")]
    public class ProductTemp
    {
        //狀態
        public enum status
        {
            未審核 = 1,
            允許上架 = 0
        }

        public ProductTemp()
        {
            //初始化
            DateTime defaultDate = DateTime.Now;
            this.Status = (byte)status.未審核;
            CreateDate = defaultDate;
            this.InvoiceType = 0;
            this.SaleType = 0;
            this.Length = 0;
            this.Width = 0;
            this.Height = 0;
            this.Weight = 0;
            this.Tax = 0;
            this.Warranty = 0;
            this.IsMarket = "N";
            this.Is18 = "N";
            this.IsShipDanger = "N";
            this.IsChokingDanger = "N";
            this.PicStart = 0;
            this.PicEnd = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? ProductID { get; set; }
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
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public Decimal? TradeTax { get; set; }
        public Decimal? Tax { get; set; }
        public int? Warranty { get; set; }
        public string UPC { get; set; }
        public string Note { get; set; }
        public string IsMarket { get; set; }
        public string Is18 { get; set; }
        public string IsShipDanger { get; set; }
        public string IsChokingDanger { get; set; }
        public string MenufacturePartNum { get; set; }
        public string SPECLabel { get; set; }
        public Decimal? SupplyShippingCharge { get; set; }

    }
}
