using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ProductPropertySketch")]
    public class ProductPropertySketch
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemSketchID { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductValueID { get; set; }

        public string UserInputValue { get; set; }
        public string UserInputValueTW { get; set; }
        public Nullable<int> Show { get; set; }
        public Nullable<int> Label { get; set; }
        public Nullable<int> GroupID { get; set; }
        public Nullable<int> PropertyNameID { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
