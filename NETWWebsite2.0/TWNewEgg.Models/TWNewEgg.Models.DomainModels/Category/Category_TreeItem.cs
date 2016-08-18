using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Category
{
    public class Category_TreeItem
    {
        //public Int32 category_sn { get; set; }
        public Int32 category_id { get; set; }
        public String category_title { get; set; }
        public String category_description { get; set; }
        public Int32 category_layer { get; set; }
        public Int32 category_parentid { get; set; }
        public Int32 category_categoryfromwsid { get; set; }
        public Int32 category_showorder { get; set; }
        public Int32 category_sellerid { get; set; }
        public Int32 category_deviceid { get; set; }
        public Int32 category_showall { get; set; }
        public Int32 category_version { get; set; }
        public String category_createuser { get; set; }
        public DateTime category_createdate { get; set; }
        public String category_updateuser { get; set; }
        public Nullable<Int32> category_updated { get; set; }
        public Nullable<DateTime> category_updatedate { get; set; }
        public Nullable<Int32> category_translatecountryid { get; set; }
        public Nullable<Int32> category_translateid { get; set; }
        public List<Category_TreeItem> Nodes { get; set; }
        public Category_TreeItem Parents { get; set; }
        public string category_manager { get; set; }
        public string ClassName { get; set; }
        public string ImagePath { get; set; }
        public string ImageHref { get; set; }
    }
}
