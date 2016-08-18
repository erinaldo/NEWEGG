using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceMaster")]
    public class InvoiceMaster
    {
        public InvoiceMaster()
        {
            DateTime defaultDate = DateTime.Parse("1900/01/01 00:00:00");
            this.InDate = DateTime.Now;
            this.EditDate = defaultDate;
        }
        [Key]
        public int SN { get; set; }
        /// <summary>
        /// 發票號碼
        /// </summary>
        public string InvoiceNo { get; set; }
        /// <summary>
        ///  發票金額
        /// </summary>
        public decimal PriceSum { get; set; }
        public decimal TaxSum { get; set; }
        public decimal ShipSum { get; set; }
        public decimal ServiceSum { get; set; }
        /// <summary>
        /// S:不拋送至WMS(ex. 三角)
        /// N:拋WMS
        /// N1:二次拋WMS
        /// N2:三次拋WMS
        /// Y:拋WMS成功
        /// Y1:N1拋WMS成功
        /// Y2:N2拋WMS成功
        /// F:拋WMS三次皆失敗
        /// </summary>
        public string SendStatus { get; set; }
        public string InUser { get; set; }
        public decimal? InstallmentFeeSum { get; set; }
        public System.DateTime InDate { get; set; }
        public string EditUser { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
    }
}