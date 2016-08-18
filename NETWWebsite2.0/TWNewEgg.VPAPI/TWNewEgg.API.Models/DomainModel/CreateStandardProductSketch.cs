using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models.DomainModel
{
    public class CreateStandardProductSketch
    {
        public CreateStandardProductSketch()
        {
            this.basicItemInfo = new List<ItemSketch>();

            this.twodimProperty = new StandProductProperty();
        }

        public List<TWNewEgg.API.Models.ItemSketch> basicItemInfo { get; set; }

        public StandProductProperty twodimProperty { get; set; }
    }
}
