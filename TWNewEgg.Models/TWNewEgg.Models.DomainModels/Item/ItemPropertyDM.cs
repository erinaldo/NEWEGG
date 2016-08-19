using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemPropertyDM
    {
        public TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup ItemPropertyGroup { get; set; }
        public TWNewEgg.Models.DomainModels.Item.ItemPropertyName ItemPropertyName { get; set; }
        public TWNewEgg.Models.DomainModels.Item.ItemPropertyValue ItemPropertyValue { get; set; }

    }
}
