using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_User")]
    public class Seller_User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserEmail { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; }
        //[ForeignKey("SellerID")]
        public Nullable<int> SellerID { get; set; }
        public string Pwd { get; set; }
        public string Status { get; set; }
        public string AccessToken { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
        public string RanNum { get; set; }
        public string RanCode { get; set; }
        public string PurviewType { get; set; }
    }
}
