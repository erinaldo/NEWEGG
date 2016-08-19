using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("OeyaIChannelsOrderInfo")]
    public class OeyaIChannelsOrderInfo
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string SalesOrderItemCode { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductQty { get; set; }
        public decimal TotalPrice { get; set; }
        public int OrderStatus { get; set; }
        public DateTime OrderStatusDate { get; set; }
        public DateTime OrderCreateDate { get; set; }
        public string OeyaInfo { get; set; }
        public string BackCode { get; set; }
        public string InvalidReason { get; set; }
        public string Other { get; set; }
        public string SendStatus { get; set; }
        public Nullable<DateTime> SendDate { get; set; }
        public Nullable<DateTime> OeyaLastQueryDate { get; set; }
    }
}
