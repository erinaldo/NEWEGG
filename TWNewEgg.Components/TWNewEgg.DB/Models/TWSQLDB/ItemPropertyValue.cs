using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemPropertyValue")]
    public  partial class ItemPropertyValue
    {
        public ItemPropertyValue()
        {
            DateTime defaultDate = DateTime.Parse("1900/01/01 00:00:00");
            Hide = "F";
            ShowOrder = 0;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PropertyNameID { get; set; }
        public string PropertyCode { get; set; }
        public string PropertyValue { get; set; }
        /// <summary>
        /// Ex.黃色
        /// </summary>
        public string PropertyValueTW { get; set; }
        public string Hide { get; set; }
        public int ShowOrder { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int? Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
