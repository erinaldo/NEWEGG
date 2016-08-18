using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models.Q4S
{
    public class ItemRaw
    {
        public string ItemNumber { get; set; }
        public int Newegg_Avail { get; set; }
        public int VF_Avail { get; set; }
        public int Canada_avail { get; set; }
        public int Canada_ExcludeAvail { get; set; }
        public int MaxQty4Promo { get; set; }
        public int ThirdPartyReserved { get; set; }
        public int CANQ4S { get; set; }
        public int USAQ4S { get; set; }
        public int B2BQ4S { get; set; }
        public int HideQ4S { get; set; }
        public bool IsCANNoShipCrossBorder { get; set; }
        public string Description { get; set; }
        public List<Warehouse> WarehouseList { get; set; }
    }
}