using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.AdvService.Models
{
    public class AdvEventRecommend
    {
        public int getType { get; set; }
        public string advHC { get; set; }
        public string recommendIDs { get; set; }
        public List<string> extraApis { get; set; }
        public List<string> extraMethods { get; set; }
        public List<string> extraArgs { get; set; }
        public List<int> extraApiNumber { get; set; }
        public int imgSize { get; set; }
        public DateTime? nowDate { get; set; }
    }
}
