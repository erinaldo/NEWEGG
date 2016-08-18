using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace TWNewEgg.DB.Models.Logistics.HiLife
{

    /// <summary>
    /// 路順路線檔
    /// </summary>
    public class HiLifeF00
    {
        private List<TWNewEgg.DB.Models.Logistics.HiLife.F00Body> m_listBody = null;
        
        public HiLifeF00()
        {
        }

        public List<F00Body> body { get; set; }
    }

    /// <summary>
    /// Hi-Life F00的 Body
    /// </summary>
    [Table("HiLifeF00Body")]
    public class F00Body
    {
        public F00Body()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// 原始店編, 長度7碼, 不足7碼右側補齊空白, 超過7碼自動截斷
        /// </summary>
        public string Stno { get; set; }

        /// <summary>
        /// 現行店編, 長度7碼, 不足7碼右側補齊空白, 超過7碼自動截斷
        /// </summary>
        public string New_Stno { get; set; }

        /// <summary>
        /// 路線路順, 長度5碼, 路線4碼+1個空白
        /// </summary>
        public string Dcrono { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }
}
