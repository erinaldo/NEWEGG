using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Models.DBModels.TWSQLDBExtModels
{
    public class DbItemProperty
    {
        public ItemPropertyGroup ItemPropertyGroup { get; set; }
        public ItemPropertyName ItemPropertyName { get; set; }
        public ItemPropertyValue ItemPropertyValue { get; set; }
    }
}
