using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("trackingcode")]
    public class TrackingCode
    {
        [Key]
        public int ID { get; set; }

        [DisplayName("Code Name")]
        public string Name { get; set; }

        [DisplayName("Code Description")]
        public string Description { get; set; }

        [DisplayName("用於哪些View")]
        public string ViewName { get; set; }

        [DisplayName("放置位置")]
        public string Position { get; set; }

        [DisplayName("Tracking Code")]
        public string TrackingCodeContent { get; set; }

        [DisplayName("Code啟用狀態")]
        public int Status { get; set; }

        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}

