using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Property
{
    public class PriceWithQty_View
    {
        public int ID { get; set; }
        public string Name { get; set; }
        // 0不顯示
        public int ShowOrder { get; set; }
        public int Qty { get; set; }
        public int? maxPrice { get; set; }
        public int? minPrice { get; set; }
    }
}
