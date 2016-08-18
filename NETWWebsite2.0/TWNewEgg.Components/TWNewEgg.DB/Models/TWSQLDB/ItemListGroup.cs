using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("itemlistgroup")]
    public class ItemListGroup
    {
        public enum type
        {
            屬性 = 10,
            一般 = 0,
            贈品 = 20
        };
        public ItemListGroup()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = DateTime.Now;
            UpdateDate = defaultDate;
            ID = 0;
            ItemID = 0;
            Name = "";
            Type = 0;
            SelectedMax = 0;
            SelectedMin = 0;
            ItemlistgroupRule = "";
            ItemlistgroupOrder = 0;
            Note = "";
        }
        //------------------------------------------------------------------------------
        [Key]
        public int ID { get; set; }
        public Nullable<int> TempID { get; set; }
        public int ItemID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int SelectedMax { get; set; }
        public int SelectedMin { get; set; }
        public string ItemlistgroupRule { get; set; }
        public int ItemlistgroupOrder { get; set; }
        public string Note { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}