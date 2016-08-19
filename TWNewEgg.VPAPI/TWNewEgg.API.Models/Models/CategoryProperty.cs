using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class PropertyGroup
    {
        public int CategoryID { get; set; }
        public int GroupID { get; set; }
    }
    public class PropertyName
    {
        public int? CategoryID { get; set; }
        public int GroupID { get; set; }
        public int PropertyID { get; set; }
        public string Name { get; set; }
    }
    public class PropertyValue
    {
        public int? PropertyID { get; set; }
        public int ValueID { get; set; }
        public string Value { get; set; }
    }
    public class PropertyResult
    {
        public int? CategoryID { get; set; }
        public int GroupID { get; set; }
        public int PropertyID { get; set; }
        public string PropertyName { get; set; }
        public List<PropertyValue> ValueInfo { get; set; }
    }
    public class SaveProductProperty
    {
        //public int ProductID { get; set; }
        public int GroupID { get; set; }
        public int PropertyID { get; set; }
        public int ValueID { get; set; }
        public string InputValue { get; set; }
        public string UpdateUser { get; set; }
    }
    public class GetProductProperty
    {
        public int ProductID { get; set; }
        public int GroupID { get; set; }
        public int PropertyID { get; set; }
        public int ValueID { get; set; }
        public string InputValue { get; set; }
    }
}
