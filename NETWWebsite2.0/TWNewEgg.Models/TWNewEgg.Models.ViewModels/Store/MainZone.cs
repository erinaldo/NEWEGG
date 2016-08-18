using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.Models.ViewModels.Store
{
    public class MainZone
    {
        public string ShowCaseTitle { get; set; }
        public string Image { get; set; }
        public List<ImageLink> LogoList { get; set; }
        public List<ShowCaseMainItem> ShowCaseItemList { get; set; }
        public List<TextLink> ShowCaseGroup { get; set; }
    }
}
