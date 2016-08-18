using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_Country")]
    public class Seller_Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public string NameTW { get; set; }
        public Nullable<int> InUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
