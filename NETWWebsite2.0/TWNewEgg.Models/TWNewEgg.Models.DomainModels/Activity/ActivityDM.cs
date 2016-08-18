using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Activity
{
    public class ActivityDM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string HtmlContext { get; set; }
        public int ShowType { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyWord { get; set; }
        public string MetaDescription { get; set; }
        public Nullable<int> ActionType { get; set; }
        public string SectionInfor { get; set; }
    }
}
