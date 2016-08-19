using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("EDI_Seller_FunctionPoint")]
    public class EDI_Seller_FunctionPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FunctionPointID { get; set; }
        public string FunctionPointName { get; set; }
        public string FunctionPointKey { get; set; }
        /// <summary>
        /// E = Enable, D = Disable
        /// </summary>
        public string Status { get; set; }
        
        public string InUserID { get; set; }
        public string InUserName { get; set; }
        public System.DateTime Indate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public string EditUserID { get; set; }
        public string EditUserName { get; set; }
        public string LanguageCode { get; set; }
        public string CompanyCode { get; set; }
        public string CountryCode { get; set; }
    }
}
