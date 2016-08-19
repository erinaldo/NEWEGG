using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Redeem
{
    public class GetItemTaxDetail
    {
        public int item_id { get; set; }
        public int itemlist_id { get; set; }
        public string pricetaxdetail { get; set; }
    }
}
