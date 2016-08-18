using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt
{
    [Table("PMImageObject")]
    public class ImageObject
    {
        [Key]
        public int ImageID { get; set; }
        public Nullable<int> AlbumImageID { get; set; }
        public string Path { get; set; }
        public string Target { get; set; }
        public string FileName { get; set; }
        public string Effect { get; set; }
        public Nullable<int> Speed { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> EffectGroupID { get; set; }
        public Nullable<int> EffectGroupOrder { get; set; }
        public string Hyperlink { get; set; }
        public string InUser { get; set; }
        public System.DateTime InDate { get; set; }
        public string LastEditUser { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
    }
}
