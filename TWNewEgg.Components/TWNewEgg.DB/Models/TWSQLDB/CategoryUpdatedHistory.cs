using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("CategoryUpdatedHistory")]
    public class CategoryUpdatedHistory
    {
        /// <summary>
        /// 修改Category,要儲存於CategoryUpdatedHistory的UpdateType
        /// </summary>
        public enum CategoryUpdatedType
        {
            Unknown = 0,
            Add_Category = 1,
            Add_Manager = 2,
            UpdatedLayer = 6,
            UpdatedBasicData = 7,
            UpdatedManager = 8
        }

        public CategoryUpdatedHistory()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// CategoryId
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 修改模式, 請參照CategoryUpdatedType
        /// </summary>
        public int UpdatedType { get; set; }

        /// <summary>
        /// 修改前的物件資料, 為JSON格式
        /// </summary>
        public string BeforeUpdatedData { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        public string CreateUser { get; set; }
    }
}
