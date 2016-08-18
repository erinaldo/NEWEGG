using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CategoryService.Models
{
    public class PropertyKey
    {
        public int PNID { get; set; }
        public string PNName { get; set; }
        public string Hide { get; set; }
        public int ShowOrder { get; set; }
        public List<PropertyValue> PropertyValues { get; set; }
    }
}