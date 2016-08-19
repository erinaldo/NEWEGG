using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Property
{
    public class SearchProperty
    {
        public int GroupID { get; set; }
        public int CategoryID { get; set; }
        public int PropertyNameID { get; set; }
        public int PropertyValueID { get; set; }
        public string GroupName { get; set; }
        public string GroupNameTW { get; set; }
        public string PropertyName { get; set; }
        public string PropertyNameTW { get; set; }
        public string PropertyValue { get; set; }
        public string PropertyValueTW { get; set; }
        public string UpdateUser { get; set; }
        public string PropertyCode { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
