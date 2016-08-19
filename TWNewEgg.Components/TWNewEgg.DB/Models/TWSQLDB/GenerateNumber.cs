using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    /// <summary>
    /// 動態生成編號的對應表
    /// </summary>
    [Table("GenerateNumberMap")]
    public class GenerateNumberMap
    {
        public GenerateNumberMap()
        {
        }

        /// <summary>
        /// 原始編號
        /// </summary>
        [Key]
        public string SourceNumber { get; set; }

        /// <summary>
        /// 動態生成的編號
        /// </summary>
        public string GenerateNumber { get; set; }

        /// <summary>
        /// 動能生成編號的時間
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime GetTime { get; set; }
    }
}
