using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("mediainfo")]
    public class MediaInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime LaunchDate { get; set; }
        public string Title { get; set; }
        public string Snapshotpath { get; set; }
        public string Clickpath { get; set; }
        public string Displaypath { get; set; }
        public int Displaytype { get; set; }
        public int ShowAll { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public Nullable<int> Updated { get; set; }
    }
}
