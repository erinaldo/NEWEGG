using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    public class NewProductData
    {
        public List<NewProduct> newproductdata { get; set; }
        public ProductData productdata { get; set; }
    }
    public class ProductData
    {
        public string productdata { get; set; }
        public string product_id { get; set; }
        public string producy_layer { get; set; }
    }
    [Table("newproduct")]
    public class NewProduct
    {
        public NewProduct()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            DateStart = defaultDate;
            DateEnd = defaultDate;
            CreateDate = defaultDate;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int CategoryID { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int CategoryLayer { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int ItemID { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int SellerID { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int ShowType { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int NewproductOrder { get; set; }
        //-------------------not null--------------------//
        [Required]
        public System.DateTime DateStart { get; set; }
        //-------------------not null--------------------//
        [Required]
        public System.DateTime DateEnd { get; set; }
        //-------------------not null--------------------//
        [Required]
        public string CreateUser { get; set; }
        //-------------------not null--------------------//
        [Required]
        public System.DateTime CreateDate { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int Updated { get; set; }
        //-------------------not null--------------------//
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}