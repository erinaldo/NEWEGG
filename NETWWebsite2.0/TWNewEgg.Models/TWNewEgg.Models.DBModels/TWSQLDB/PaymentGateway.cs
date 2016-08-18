using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    /// <summary>
    /// 金流
    /// </summary>
    [Table("PaymentGateway")]
    public class PaymentGateway
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 金流名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        /// <remarks>enum PaymentGatewayStatus</remarks>
        public int Status { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        public System.DateTime CreateDate { get; set; }
        
        /// <summary>
        /// 更新日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }
        
        /// <summary>
        /// 建立人
        /// </summary>
        public string CreateUser { get; set; }
        
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUser { get; set; }
    }

    /// <summary>
    /// 金流狀態
    /// </summary>
    public enum PaymentGatewayStatus
    { 
        // 關閉
        Disable = 0,

        // 啟用
        Enable = 1
    }
}
