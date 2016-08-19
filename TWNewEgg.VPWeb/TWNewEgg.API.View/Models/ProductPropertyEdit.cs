using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.View
{
    public class ProductPropertyEdit
    {
        public int? CategoryID { get; set; }

        public int GroupID { get; set; }

        public int PropertyID { get; set; }

        public string PropertyName { get; set; }

        public List<PropertyValue> ValueInfo { get; set; }

        public int ValueOption { get; set; }

        public int ValueID { get; set; }

        public string InputValue { get; set; }
    }
}