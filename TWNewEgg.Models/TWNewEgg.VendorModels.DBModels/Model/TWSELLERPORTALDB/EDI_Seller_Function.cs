using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("EDI_Seller_Function")]
    public class EDI_Seller_Function
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FunctionID { get; set; }
        public string FunctionName { get; set; }
        public int Order { get; set; }
        public string IsShowOnDesktop { get; set; }

        /// <summary>
        /// E = Enable, D = Disable
        /// </summary>
        public string Status { get; set; }

        public string Url { get; set; }
        public int CategoryID { get; set; }

        public string InUserID { get; set; }
        public string InUserName { get; set; }
        public System.DateTime Indate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public string EditUserID { get; set; }
        public string EditUserName { get; set; }
        public string LanguageCode { get; set; }
        public string CompanyCode { get; set; }
        public string CountryCode { get; set; }

        /// <summary>
        /// Y = true, N = false
        /// </summary>
        public string IsRelease { get; set; }

        /// <summary>
        /// S = Seller function, I = Internal function
        /// </summary>
        public string FunctionType { get; set; }
    }
}
