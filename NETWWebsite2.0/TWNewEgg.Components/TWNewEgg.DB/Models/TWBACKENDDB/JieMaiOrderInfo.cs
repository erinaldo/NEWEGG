using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("JieMaiOrderInfo")]
    public class JieMaiOrderInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string SalesOrderCode { get; set; }
        public int UploadFlag { get; set; }
        public string OrderID { get; set; }
        public string JMOrderID { get; set; }
        public Nullable<decimal> TotalFee { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public Nullable<int> CartStatus { get; set; }
        public Nullable<DateTime> CartCreateDate { get; set; }
    }
}
