using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("landingpageList")]
    public class LandingPageList
    {
        public LandingPageList()
        {
            this.ShowType = 0;
            this.GetMore = 0;
            this.Updated = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public string ItemList { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public int ShowType { get; set; }
        public int GetMore { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
