using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("salesordercancel")]
    public class SalesOrderCancel
    {
        public SalesOrderCancel()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
        }
        [Key]
        public int ID { get; set; }
        public string SalesorderCode { get; set; }
        public int ItemID { get; set; }
        public string CauseNote { get; set; }
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string AccountNO { get; set; }
        public string AccountName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string CreateUser { get; set; }//2014.03.10 penny
    }
}