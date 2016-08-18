using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("category")]
    public class Category
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Layer { get; set; }
        public int ParentID { get; set; }
        public int CategoryfromwsID { get; set; }
        public int Showorder { get; set; }
        public int SellerID { get; set; }
        public int DeviceID { get; set; }
        public int ShowAll { get; set; }
        public int VerSion { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> TranslateCountryID { get; set; }
        public Nullable<int> TranslateID { get; set; }
        public decimal GrossMargin { get; set; }
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
        /// <summary>
        /// 副標題
        /// </summary>
        public string SubTitle { get; set; }
    }
}
