using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Item
{
    public class ItemGroupProperty
    {
        public int GroupID { get; set; }
        public int PropertyID { get; set; }
        public int Order { get; set; }
        public string PropertyName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int InUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
    }
}
