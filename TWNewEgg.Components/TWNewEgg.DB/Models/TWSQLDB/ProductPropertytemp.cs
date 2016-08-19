using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ProductPropertytemp")]
    public partial class ProductPropertytemp
    {
        public enum fronShow
        {
            Notshowfront = 1,
            showfront = 0

        }
        public ProductPropertytemp()
        {
            UpdateDate = DateTime.Now;
        }
        [Key, Column("producttempID", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "producttempID，商品暫存ID為必填欄位")]
        public int producttempID { get; set; }
        public int? ProductID { get; set; }
        [Key, Column("ProductValueID", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "ProductValueID，屬性值ID為必填欄位")]
        public int ProductValueID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "GroupID，群組ID為必填欄位")]
        public int GroupID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "PropertyNameID，屬性名稱ID為必填欄位")]
        public int PropertyNameID { get; set; }
        public string UserInputValue { get; set; }
        public string UserInputValueTW { get; set; }
        public int? Show { get; set; }
        public int? Label { get; set; }
        [Required(ErrorMessage = "CreateUser，建立者為必填欄位")]
        public string CreateUser { get; set; }
        [Required(ErrorMessage = "CreateDate，建立者為必填欄位")]
        public Nullable<DateTime> CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
