using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
   
        //
        // GET: /delivery/
       [Table("delivery")]
        public partial class Delivery
        {
            [Key]
            public int ID { get; set; }
            public string Code { get; set; }
            public Nullable<int> ShipperID { get; set; }
            public Nullable<int> SpID { get; set; }
            public string Receiver { get; set; }
            public string ZipCode { get; set; }
            public string ADDR { get; set; }
            public string Phone { get; set; }
            public string Phone2 { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public Nullable<System.DateTime> Date { get; set; }
            public Nullable<System.DateTime> FinalDate { get; set; }
            public string DeliveryUser { get; set; }
            public string EndUser { get; set; }
            public Nullable<int> Status { get; set; }
            public string EcanCode { get; set; }
            public string Location { get; set; }
            public Nullable<int> Carton { get; set; }
            public Nullable<int> Size { get; set; }
            public Nullable<int> Gmt { get; set; }
            public Nullable<decimal> Cod { get; set; }
            public string Filename { get; set; }
            public string Note { get; set; }
           // public Nullable<System.DateTime> SysDate { get; set; }
            public Nullable<int> Updated { get; set; }
            public Nullable<System.DateTime> UpdateDate { get; set; }
            public string UpdateUser { get; set; }
            public string PurchaseorderitemCode { get; set; }
       
    }
}
