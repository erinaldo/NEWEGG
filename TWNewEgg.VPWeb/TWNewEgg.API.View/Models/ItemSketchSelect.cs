using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TWNewEgg.API.View
{
    public class ItemSketchSelect: TWNewEgg.API.Models.ItemSketch
    {
        [UIHint("ClientCategory")]
        public CategoryViewModel Category
        {
            get;
            set;
        }
        [UIHint("ClientDate")]
        public DateTime? startDateSketch { get; set; }
    }
    public class CategoryViewModel
    {
        public string shiptype { get; set; }
        public string shiptypeCode { get; set; }
    }
    public class itemsketchPropertyExamine : TWNewEgg.API.Models.ItemSketch
    {
        [UIHint("ClientCategory")]
        public CategoryViewModel Category
        {
            get;
            set;
        }
        [UIHint("ClientDate")]
        public DateTime? startDate { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public int groupid { get; set; }
        public string inputValue { get; set; }
    }
}