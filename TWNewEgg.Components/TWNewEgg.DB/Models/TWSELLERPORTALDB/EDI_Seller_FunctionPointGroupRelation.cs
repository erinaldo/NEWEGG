using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("EDI_Seller_FunctionPointGroupRelation")]
    public class EDI_Seller_FunctionPointGroupRelation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TransactionNumber { get; set; }
        public int FunctionPointGroupID { get; set; }
        public int FunctionPointID { get; set; }

        public string InUserID { get; set; }
        public string InUserName { get; set; }
        public System.DateTime Indate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public string EditUserID { get; set; }
        public string EditUserName { get; set; }
    }
}
