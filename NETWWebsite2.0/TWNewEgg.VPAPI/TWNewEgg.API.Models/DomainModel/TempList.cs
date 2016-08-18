using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models.DomainModel
{
    public class TempList
    {
        public int UpdateUserID { get; set; }

        public int ItemID { get; set; }

        public int ItemTempID { get; set; }

        public int ProductID { get; set; }

        public int ProductTempID { get; set; }

        public string ShipType { get; set; }

        public int DelvType { get; set; }

        public decimal MarketPrice { get; set; }

        public decimal Cost { get; set; }

        public decimal PriceCash { get; set; }

        public DateTime DateSatrt { get; set; }

        public int Qty { get; set; }

        public int ItemQty { get; set; }

        public int SafeQty { get; set; }
    }
}
