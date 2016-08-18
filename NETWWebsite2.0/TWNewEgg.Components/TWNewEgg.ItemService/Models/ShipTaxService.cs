using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;

namespace TWNewEgg.ItemService.Models
{
    public class ShipTaxService
    {
        public Dictionary<string, decimal> ShippingCost { get; set; }
        public Dictionary<string, List<GetItemTaxDetail>> ShippingTaxCost { get; set; }
    }
}
