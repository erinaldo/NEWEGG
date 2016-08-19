using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("contactaddress")]
    public class ContactAddress
    {
        public ContactAddress()
        {
            this.PrimaryCode = "N";
            this.InDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public int SellerID { get; set; }
        public string PrimaryCode { get; set; }
        public int ContactTypeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneRegion { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CountryCode { get; set; }
        public string ZipCode { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime InDate { get; set; }
        public string InUser { get; set; }
    }
}
