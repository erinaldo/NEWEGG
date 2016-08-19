using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("hotsale")]
    public class HotSale
    {
        public HotSale()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            DateStart = defaultDate;
            DateEnd = defaultDate;
            CreateDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int CategoryID { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int CategoryLayer { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int ItemID { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int SellerID { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int ShowType { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int HotsaleOrder { get; set; }
        //-------------------not null--------------------//
        [Required]
        public System.DateTime DateStart { get; set; }
        //-------------------not null--------------------//
        [Required]
        public System.DateTime DateEnd { get; set; }
        //-------------------not null--------------------//
        [Required]
        public string Createuser { get; set; }
        //-------------------not null--------------------//
        [Required]
        public System.DateTime CreateDate { get; set; }
        //-------------------not null--------------------//
        [Required]
        public int Updated { get; set; }
        //-------------------not null--------------------//
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }


}