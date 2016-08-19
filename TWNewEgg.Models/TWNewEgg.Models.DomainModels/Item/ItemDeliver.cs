using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemDeliver
    {
        public ItemDeliver()
        {
            this.itemBlack = new List<DBModels.TWSQLDB.ItemDeliverBlack>();
            this.itemWhite = new List<DBModels.TWSQLDB.ItemDeliverWhite>();
        }
        public List<TWNewEgg.Models.DBModels.TWSQLDB.ItemDeliverBlack> itemBlack { get; set; }
        public List<TWNewEgg.Models.DBModels.TWSQLDB.ItemDeliverWhite> itemWhite { get; set; }
    }
}
