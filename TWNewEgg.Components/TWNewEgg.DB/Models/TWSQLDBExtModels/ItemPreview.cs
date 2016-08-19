using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSQLDB.Models.ExtModels
{
    public class ItemPreview
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Spechead { get; set; }
        public decimal PriceCash { get; set; }
        public string Specdetail { get; set; }
        public string Photoname { get; set; }
        public int? ProductID { get; set; }
        public int? ProductFK { get; set; }
        public int? Rating { get; set; }
        public int? TotalReviews { get; set; }
        public int? SellingQty { get; set; }
        public decimal PriceGlobalship { get; set; }

        public ItemPreview() { }
        public ItemPreview(TWNewEgg.DB.TWSQLDB.Models.Item item)
        {
            ID = item.ID;
            Name = item.Name;
            Spechead = item.Spechead;
            PriceCash = item.PriceCash;
            PriceGlobalship = item.PriceGlobalship;
        }

        public ItemPreview(TWNewEgg.DB.TWSQLDB.Models.ItemList itemlist)
        {
            ID = itemlist.ID;
            Name = itemlist.Name;
            Spechead = itemlist.Sdesc;
            PriceCash = itemlist.Price;
        }
    }
}
