using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.Models.Logistics.HiLife
{

    /// <summary>
    /// 取消路線順檔
    /// </summary>
    public class HiLifeTxt
    {

        public HiLifeTxt()
        { 
        }
      
        public List<TxtBody> body { get; set; }
        
    }

    [Table("HiLifeTxtBody")]
    public class TxtBody
    {
        public TxtBody()
        { 
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 原始店碼, 長度7位元, 不足7位元, 右側補齊空白
        /// </summary>
        public string Stno { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }
    
}
