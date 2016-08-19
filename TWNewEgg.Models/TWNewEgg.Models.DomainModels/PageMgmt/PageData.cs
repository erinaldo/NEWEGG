using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;

namespace TWNewEgg.Models.DomainModels.PageMgmt
{
    public class PageData
    {
        public PageInfo page { get; set; }
        public List<ComponentInfo> editComponents { get; set; }
        public List<TextObject> texts { get; set; }
        public List<VideoInfo> videos { get; set; }
        public List<ImageObject> images { get; set; }
        public List<DynamicObject> dynamics { get; set; }
    }
}
