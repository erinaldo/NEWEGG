using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("PlaceOrderRecord")]
    public class PlaceOrderRecord
    {
        public PlaceOrderRecord()
        {
            this.IsSuccess = false;
            this.CreateDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string PurchaseOrderItemCode { get; set; }
        public string SellerOrderCode { get; set; }
        public int TradeMode { get; set; }
        public string SellerProductID { get; set; }
        public int Qty { get; set; }
        public Nullable<decimal> OriginalUnitPrice { get; set; }
        public Nullable<decimal> CurrentUnitPrice { get; set; }
        public Nullable<decimal> OriginalShippingCharge { get; set; }
        public Nullable<decimal> CurrentShippingCharge { get; set; }
        public Nullable<decimal> OriginalAmount { get; set; }
        public Nullable<decimal> CurrentAmount { get; set; }
        public string ShipViaCode { get; set; }
        public string Warehouse { get; set; }
        public bool IsSuccess { get; set; }
        public Nullable<DateTime> LastSendDate { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
    }
}
