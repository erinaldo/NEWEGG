using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt
{
    [Table("PMPageInfo")]
    public class PageInfo
    {
        public class PageStatus
        {
            public static string Editing = "E";
            public static string Waiting = "W";
            public static string Reject = "R";
            public static string Active = "A";
            public static string Deactive = "D";
        }
        public string InUser { get; set; }
        public System.DateTime InDate { get; set; }
        public string LastEditUser { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
        [Key]
        public int PageID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> PageOrder { get; set; }
        public string Status { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Width { get; set; }
        public string BackgroundImg { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }

    }
}
