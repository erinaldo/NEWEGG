using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Property
{
    public class PropertyGroup_View
    {
        public PropertyGroup_View() {
            this.GroupProperties = new List<PropertyKey_View>();
        }
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public string Hide { get; set; }
        public int ShowOrder { get; set; }
        public List<PropertyKey_View> GroupProperties { get; set; }
    }
}
