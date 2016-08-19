using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSQLDB.Models.ExtModels
{
    public class ItemPropertiesGroup
    {

        public string GroupName { get; set; }
        public int GroupID { get; set; }
        public List<ItemXMLProperties> ItemProperties { get; set; }
    }
}
