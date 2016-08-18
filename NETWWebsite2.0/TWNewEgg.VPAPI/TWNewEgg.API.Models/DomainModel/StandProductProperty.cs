using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;

namespace TWNewEgg.API.Models.DomainModel
{
    public class StandProductProperty
    {
        public StandProductProperty()
        {
            this.ItemGroup = new List<ItemSketchGroup>();

            this.ItemProperty = new List<ItemSketchProperty>();
        }

        public List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchGroup> ItemGroup { get; set; }

        public List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> ItemProperty { get; set; }
    }

    
}
