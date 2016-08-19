using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("companybook")]
    public class CompanyBook
    {
        public CompanyBook()
        {
            Createdate = DateTime.Now;
            Updated = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Accountid { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        public string Number { get; set; }
        public string Delivloc { get; set; }
        public string Delivzip { get; set; }
        public string Delivaddr { get; set; }
        public System.DateTime Createdate { get; set; }
        public int Updated { get; set; }
        public Nullable<System.DateTime> Updatedate { get; set; }
    }
}