using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("categoryfromws")]
    public class CategoryFromWS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string NValue { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Layer { get; set; }
        public int ParentID { get; set; }
        public int Showorder { get; set; }
        public int SellerID { get; set; }
        public int DeviceID { get; set; }
        public int ShowAll { get; set; }
        public int Version { get; set; }
    }
}