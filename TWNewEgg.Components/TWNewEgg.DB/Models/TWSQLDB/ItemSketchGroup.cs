using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemSketchGroup")]
    public class ItemSketchGroup
    {
        public ItemSketchGroup()
        {
            CreateDate = DateTime.Now;
        }

        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemSketchID { get; set; }
    
        public int? PropertyID { get; set; }

        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Order { get; set; }
        public string PropertyName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int InUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
    }
}
