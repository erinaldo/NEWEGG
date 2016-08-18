using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("CategoryAssociatedWithPM")]
    public class CategoryAssociatedWithPM
    {
        public CategoryAssociatedWithPM()
        {
            Delvtype = "f";
        }

        /// <summary>
        /// 流水號
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// Category ID
        /// </summary>
        [Key]
        [Column(Order = 0)]
        public int CategoryID { get; set; }

        /// <summary>
        /// Deliver Type
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public string Delvtype { get; set; }

        /// <summary>
        /// Manager的顯示名稱
        /// </summary>
        public string ProductMananeger { get; set; }

        /// <summary>
        /// Manager的Login短名
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public string ManagerName { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改次數
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int Updated { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 是否啟用此Manager權限, 0為不啟用, 1為啟用
        /// </summary>
        public int ActiveStatus { get; set; }
    }
}
