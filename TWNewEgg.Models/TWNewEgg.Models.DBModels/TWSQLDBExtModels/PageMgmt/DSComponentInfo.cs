using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;

namespace TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt
{
    [NotMapped]
    public class DSComponentInfo : ComponentInfo
    {
        public List<ImageObject> Image { set; get; }
        public VideoInfo Video { get; set; }
        public TextObject Text { get; set; }
        public DynamicObject Dynamic { get; set; }
    }
}
