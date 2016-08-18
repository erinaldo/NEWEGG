using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Property
{
    public class PropertyKey_View
    {
        public PropertyKey_View()
        {
            this.PropertyValues = new List<PropertyValue_View>();
        }
        public int PNID { get; set; }
        public string PNName { get; set; }
        public string Hide { get; set; }
        public int ShowOrder { get; set; }
        public List<PropertyValue_View> PropertyValues { get; set; } 
    }
}
