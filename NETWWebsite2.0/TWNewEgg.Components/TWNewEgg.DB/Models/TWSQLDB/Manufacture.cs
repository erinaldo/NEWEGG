using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("manufacture")]
    public class Manufacture
    {
        public Manufacture()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
        }
        /// <summary>
        /// 製造商ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// 製造商名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 填90000
        /// </summary>
        public int? Showorder { get; set; }
        /// <summary>
        /// 填 0
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 建立者
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public System.DateTime CreateDate { get; set; }
        /// <summary>
        /// 填 0
        /// </summary>
        public int Updated { get; set; }
        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdateUser { get; set; }
        public string Phone { get; set; }
        public string WebAddress { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// null
        /// </summary>
        public string SourceContry { get; set; }
        /// <summary>
        /// 品牌故事
        /// </summary>
        public string BrandStory { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public Nullable<System.DateTime> Updatedate { get; set; }
    }
}