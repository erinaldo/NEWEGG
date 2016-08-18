using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Store
{
    public class CategoryPage_View
    {
        public List<PullDownBanner> PullDownBanner { get; set; }
        public List<ImageLink> PullDownAdv { get; set; }
        public ImageLink SingleBanner { get; set; }
        public List<string> ShowCaseElevator { get; set; }
        public List<ShowCase> ShowCase { get; set; }

    }
}
