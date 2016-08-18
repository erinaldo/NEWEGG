using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceTrack")]
    public class InvoiceTrack
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public int Year { get; set; }
        public int BeginMonth { get; set; }
        public int EndMonth { get; set; }
        public string InvoiceTrackName { get; set; }
        public int BeginNumber { get; set; }
        public int EndNumber { get; set; }
        public int CurrentNumber { get; set; }
    }
}
