using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("CouponActivity")]
    public class CouponActivity
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
 
        
        /// <summary>
        /// 有效起始日
        /// </summary>
        public DateTime ActivityStart { get; set; }

        /// <summary>
        /// 有效截止日
        /// </summary>
        public DateTime ActivityEnd { get; set; }

        /// <summary>
        /// 對應的活動序號
        /// </summary>
        public int EventID { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public int Qty { get; set; }
    }
}
