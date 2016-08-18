using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_BulletinsMessage")]
    public class Seller_BulletinsMessage
    {
        public Seller_BulletinsMessage()
        {
            this.Enable = "N";
            this.UpdateDate = DateTime.Now;
        }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "請填入系統別")]
        public string FromSystem { get; set; }
        public string Enable { get; set; }
        public string MessageContent { get; set; }
        public int Updated { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public System.DateTime InDate { get; set; }
        public string InUser { get; set; }
    }
}
