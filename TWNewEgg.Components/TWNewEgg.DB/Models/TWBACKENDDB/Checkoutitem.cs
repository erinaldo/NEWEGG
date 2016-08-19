using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Checkoutitem")]
    public class Checkoutitem
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("checkout")]
        public Nullable<int> CheckoutID { get; set; }
        public virtual Checkout checkout { get; set; }

        public string SoCode { get; set; }
        public string SoitemCode { get; set; }
        public Nullable<int> ProductID { get; set; }
        public string Attribs { get; set; }
        public int Qty { get; set; }
        public Nullable<int> QtyBad { get; set; }
        public Nullable<int> QtyDead { get; set; }
        public decimal Price { get; set; }
        public DateTime? Stckdate { get; set; }
        public string Stckuser { get; set; }
        public DateTime? EraseDate { get; set; }
        public Nullable<int> EraseCause { get; set; }
        public string EraseCauseNote { get; set; }
        public DateTime? CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}