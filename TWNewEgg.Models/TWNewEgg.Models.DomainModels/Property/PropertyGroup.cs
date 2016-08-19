using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Property
{
    public class PropertyGroup
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public string Hide { get; set; }
        public int ShowOrder { get; set; }
        public List<PropertyKey> GroupProperties { get; set; }
    }

}
