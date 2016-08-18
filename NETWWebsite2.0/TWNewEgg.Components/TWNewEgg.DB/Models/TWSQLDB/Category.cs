using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("category")]
    public class Category
    {
        public Category()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
            UpdateDate = defaultDate;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// 英文名稱
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 中文名稱
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 層級
        /// </summary>
        public int Layer { get; set; }
        /// <summary>
        /// 上一層的category ID
        /// </summary>
        public int ParentID { get; set; }
        public int CategoryfromwsID { get; set; }
        public int Showorder { get; set; }
        /// <summary>
        /// 供應商ID
        /// </summary>
        public int SellerID { get; set; }
        public int DeviceID { get; set; }
        /// <summary>
        /// 0 不顯示 1顯示
        /// </summary>
        public int ShowAll { get; set; }
        public int VerSion { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> TranslateCountryID { get; set; }
        public Nullable<int> TranslateID { get; set; }

        /// <summary>
        /// 毛利率
        /// </summary>
        public decimal GrossMargin { get; set; }
        /// <summary>
        /// 套用的CSS
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 是否為手機
        /// </summary>
        public int? IsMobile { get; set; }

        /// <summary>
        /// 手機排序
        /// </summary>
        public int? MobileOrder { get; set; }

        public string ImagePath { get; set; }

        public string ImageHref { get; set; }
    }
}