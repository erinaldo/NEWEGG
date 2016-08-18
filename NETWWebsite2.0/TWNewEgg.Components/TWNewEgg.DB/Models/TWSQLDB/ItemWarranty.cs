using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemWarranty")]
    public class ItemWarranty
    {
        public ItemWarranty()
        {
            Updated = 0;
            DateTime _defaultTime = DateTime.Now;
            Createdate = _defaultTime;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Itemid { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> Update { get; set; }
        public Nullable<int> Updated { get; set; }
        public string Updateuser { get; set; }
        public int ProductProperty { get; set; }
        public int Desc { get; set; }
    }
}
