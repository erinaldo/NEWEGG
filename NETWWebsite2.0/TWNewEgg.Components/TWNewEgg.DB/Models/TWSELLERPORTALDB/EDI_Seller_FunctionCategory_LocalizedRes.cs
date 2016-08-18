using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("EDI_Seller_FunctionCategory_LocalizedRes")]
    public class EDI_Seller_FunctionCategory_LocalizedRes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TransactionNumber { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> ReferenceCategoryID { get; set; }
        public string MenuDescription { get; set; }
        public string IconStyle { get; set; }
        public string LinkPath { get; set; }
        public string LanguageCode { get; set; }
        public string InUserID { get; set; }
        public System.DateTime Indate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public string EditUserID { get; set; }
    }
}
