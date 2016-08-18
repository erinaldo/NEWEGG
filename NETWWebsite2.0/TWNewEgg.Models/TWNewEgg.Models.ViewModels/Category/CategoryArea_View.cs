using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.ViewModels.Category
{
    public class CategoryArea_View
    {
        //public Int32 category_sn { get; set; }
        public TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View CategoryItemInfoMain_View { get; set; }
        public List<TWNewEgg.Models.ViewModels.Property.PropertyGroup_View> PropertyGroup_ViewList { get; set; }
        /// <summary>
        /// 小廣告Banner,有可能為空值.
        /// </summary>
        public List<StoreBanner> PullDownAdvList { get; set; }
        public string BreadString { get; set; }
    }
}
