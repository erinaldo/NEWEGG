using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("groupbuy")]
    public class GroupBuy
    {
        public GroupBuy()
        {
            this.PromoText = "";
            this.ImgUrl = "";
            this.RejectCause = "";
            this.InUser = "";
            this.InDate = DateTime.UtcNow.AddHours(8);
        }
        
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal GroupBuyPrice { get; set; }
        public decimal ProductCost { get; set; }
        public decimal ShippingCost { get; set; }
        public int SalesOrderLimit { get; set; }
        public int SalesOrderBase { get; set; }
        public bool IsExclusive { get; set; }
        public bool IsNeweggUSASync { get; set; }
        public string PromoText { get; set; }
        public string ImgUrl { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsWaitingForApprove { get; set; }
        public bool IsApprove { get; set; }
        public bool IsReject { get; set; }
        public string RejectCause { get; set; }
        public bool IsHide { get; set; }
        public string InUser { get; set; }
        public DateTime InDate { get; set; }
        public string EditUser { get; set; }
        public Nullable<DateTime> EditDate { get; set; }
    }
}
