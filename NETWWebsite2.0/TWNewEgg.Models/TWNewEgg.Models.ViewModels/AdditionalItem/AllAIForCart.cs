using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.ViewModels.Item;

namespace TWNewEgg.Models.ViewModels.AdditionalItem
{
    public class AllAIForCart
    {
        public ItemBasic ItemBasic { get; set; }
        public List<ItemMarketGroup> itemGroup { get; set; }
        public AIForCartDM additionalItemForCart { get; set; }
    }
}
