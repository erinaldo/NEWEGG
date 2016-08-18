using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("itemproperty")]
    public class ItemProperty
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public int PropertyCode { get; set; }
        public string PropertyName { get; set; }
        public int ValueCode { get; set; }
        public string ValueName { get; set; }
        public string UserInputted { get; set; }
        public string Disabled { get; set; }
        public string GroupName { get; set; }
        public string ItemNumber { get; set; }
        public string ValueNameTW { get; set; }
        public string UserInputtedTW { get; set; }
        public string GroupNameTW { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<DateTime> EditDate { get; set; }
        public string EditUser { get; set; }
    }
}
