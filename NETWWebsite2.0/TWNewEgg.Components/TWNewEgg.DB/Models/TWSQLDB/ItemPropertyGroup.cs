using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemPropertyGroup")]
    public  partial class ItemPropertyGroup
    {
        public ItemPropertyGroup()
        {
            DateTime defaultDate = DateTime.Parse("1900/01/01 00:00:00");
            Hide = "F";
            ShowOrder = 0;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public int ID { get; set; }

        /// <summary>
        /// 第三層類別ID 
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? CategoryID { get; set; }
        /// <summary>
        /// 外觀EX:Model, Spec, Features, Size
        /// </summary>
        public string GroupName { get; set; }
        public string GroupNameTW { get; set; }
        public string Hide { get; set; }
        public int ShowOrder { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int? Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
