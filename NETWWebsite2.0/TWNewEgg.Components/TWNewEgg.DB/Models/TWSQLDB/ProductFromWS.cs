using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("productfromws")]
    public class ProductFromWS
    {
        //狀態
        public enum status
        {
            未審核缺CCC = 1,
            未審核有CCC = 2,
            完成選品 = 3,
            編輯完成 = 4,
            略過 = 5,
            Recertified = 6,
            Restricted = 7,
            OpenBox = 8,
            允許上架 = 0,
            管制商品 = 9
        };

        public ProductFromWS()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ItemNumber { get; set; }
        public string Name { get; set; }
        public string NameTW { get; set; }
        public string Description { get; set; }
        public string DescriptionTW { get; set; }
        public string SPEC { get; set; }
        public int ManufactureID { get; set; }
        public string Model { get; set; }
        public string BarCode { get; set; }
        public Nullable<int> SellerID { get; set; }
        public Nullable<int> DelvType { get; set; }
        public Nullable<int> PicStart { get; set; }
        public Nullable<int> PicEnd { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public int Status { get; set; }
        public string NValue { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<int> SaleType { get; set; }
        public string CCC { get; set; }
        public string HS { get; set; }
        public Nullable<int> Rating { get; set; }
        public Nullable<int> TotalReviews { get; set; }
        public string ImagePath { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> Width { get; set; }
        public Nullable<decimal> Height { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> Qty { get; set; }
        public string IsFirstFromAsia { get; set; }
        public string SellerName { get; set; }
    }
}