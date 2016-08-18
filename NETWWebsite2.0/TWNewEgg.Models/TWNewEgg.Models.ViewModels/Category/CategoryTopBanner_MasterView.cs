using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Category
{
    public class CategoryTopBanner_MasterView
    {
        public CategoryTopBanner_MasterView()
        {
            this.NewCategoryTopBanner = new List<CategoryTopBanner_View>();
            this.OldCategoryTopBanner = new List<TWNewEgg.Models.DomainModels.Store.MainZone>();
        }
        public List<CategoryTopBanner_View> NewCategoryTopBanner { get; set; }
        public List<TWNewEgg.Models.DomainModels.Store.MainZone> OldCategoryTopBanner { get; set; }
    }
}
