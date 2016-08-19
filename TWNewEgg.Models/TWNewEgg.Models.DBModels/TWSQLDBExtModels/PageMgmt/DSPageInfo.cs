using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;

namespace TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt
{
    [NotMapped]
    public class DSPageInfo : PageInfo
    {
        public List<DSComponentInfo> ComponentInfo { get; set; }
    }
}
