using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt
{
    [Table("PMVideoInfo")]
    public class VideoInfo
    {
        [Key]
        public int VideoID { get; set; }
        public int DarenID { get; set; }
        public string ProviderVideoID { get; set; }
        public int VideoCategoryID { get; set; }
        public string Status { get; set; }
        public string Duration { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProviderID { get; set; }
        public string ChannelTitle { get; set; }
        public int ViewCount { get; set; }
        public System.DateTime PublishedAt { get; set; }
        public string InUser { get; set; }
        public System.DateTime InDate { get; set; }
        public string LastEditUser { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
    }
}
