using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class Seller_FunctionCategoryLocalized
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CompanyCode { get; set; }
        public string CountryCode { get; set; }
        public DateTime? EditDate { get; set; }
        public string EditUserID { get; set; }
        public string EditUserName { get; set; }
        public string IconStyle { get; set; }
        public DateTime Indate { get; set; }
        public string InUserID { get; set; }
        public string InUserName { get; set; }
        public bool IsRelease { get; set; }
        public string LanguageCode { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }
        public int ParentCategoryID { get; set; }
        public string Status { get; set; }
    }
}
