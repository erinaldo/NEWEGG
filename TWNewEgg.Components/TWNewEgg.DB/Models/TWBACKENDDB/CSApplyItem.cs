using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("CSApplyItem")]
    public class CSApplyItem
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string Code { get; set; }
        public string CSApplyCode { get; set; }
        public int ProductID { get; set; }
        public int SellerID { get; set; }
        public string ProductName { get; set; }
        public string UPC { get; set; }
        public int Qty { get; set; }
        public string Manufacturers { get; set; }
        public string Model { get; set; }
        public string EnumMarketplace { get; set; }
        public string EnumCSUint { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string FullImageName { get; set; }
        public string TrackingNumber { get; set; }
        //public DateTime? CheckInDate { get; set; }
    }
}
