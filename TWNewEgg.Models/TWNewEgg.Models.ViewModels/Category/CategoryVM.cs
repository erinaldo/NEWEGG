using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Category
{
    class CategoryVM
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Layer { get; set; }
        public int? ParentID { get; set; }
        public int? CategoryfromwsID { get; set; }
        public int? Showorder { get; set; }
        public int? SellerID { get; set; }
        public int? DeviceID { get; set; }
        public int? ShowAll { get; set; }
        public int? VerSion { get; set; }
        public string UpdateUser { get; set; }
        public int? Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public int? TranslateCountryID { get; set; }
        public int? TranslateID { get; set; }
        public decimal GrossMargin { get; set; }
        public string ClassName { get; set; }
        public int? IsMobile { get; set; }
        public int? MobileOrder { get; set; }
        public string ImagePath { get; set; }
        public string ImageHref { get; set; }
        /// <summary>
        /// categorybanner副標題
        /// </summary>
        public string SubTitle { get; set; }
    }
}
