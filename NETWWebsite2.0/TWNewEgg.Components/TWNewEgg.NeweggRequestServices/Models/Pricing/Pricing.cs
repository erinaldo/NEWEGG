using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.NeweggUSARequestServices.Models.Pricing
{

    public class Response
    {
        public List<ItemInfo> Items { get; set; }
    }

    public class ItemInfo
    {
        public string ItemNumber { get; set; }
        public bool Available { get; set; }
        public decimal FinalPrice { get; set; }
        public int VFInventory { get; set; }
        public decimal ShippingCharge { get; set; }
        public List<Warehouse> Warehouses { get; set; }
    }

    public class Warehouse
    {
        public string WHNo { get; set; }
        public int Quantity { get; set; }
    }
}
