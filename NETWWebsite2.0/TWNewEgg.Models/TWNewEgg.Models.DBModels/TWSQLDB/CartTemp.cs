using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("carttemp")]
    public class CartTemp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string SerialNumber { get; set; }
        public int AccountID { get; set; }
        public Nullable<int> PayType { get; set; }
        public Nullable<int> BankID { get; set; }
        public Nullable<int> PayTypeGroupID { get; set; }
        public Nullable<int> CartTypeID { get; set; }
        public Nullable<int> SalesOrderGroupID { get; set; }
        public int Status { get; set; }
        public string IPAddress { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
