using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.EDM
{
    public class EDMBookVM
    {
        public int ID { get; set; }
        public string EDMName { get; set; } // EDM顯示名稱
        public string HtmlContext { get; set; }
    }
}
