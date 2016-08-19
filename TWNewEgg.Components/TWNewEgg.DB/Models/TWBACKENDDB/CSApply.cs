using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("CSApply")]
    public class CSApply
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string Code { get; set; }
        public int ApplicantCompanyID { get; set; }
        public string ApplicantCompanyName { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string EnumCSSellerVendor { get; set; }
        public string ShipLocationName { get; set; }
        public int ShipLocationApplicantCompanyID { get; set; }
        public string ShipLocationContact { get; set; }
        public string ShipLocationPhone { get; set; }
        public string ShipLocationCountry { get; set; }
        public string ShipLocationState { get; set; }
        public string ShipLocationCity { get; set; }
        public string ShipLocationAddress { get; set; }
        public string ShipLocationZipCode { get; set; }
        public string DeliveryID { get; set; }
        public string DeliveryCompanyID { get; set; }
        public int DeliveryBoxQuantity { get; set; }
        public string DeliveryCompanyOther { get; set; }
        public string ConsignmentStockLocationID { get; set; }
        public string EnumCSSellTWNewEgg { get; set; }
        public int? InventoryStatus { get; set; }
        public DateTime? InventoryStatusDate { get; set; }
        public string InventoryStatusUser { get; set; }
        public DateTime? CheckInDate { get; set; }
    }
}
