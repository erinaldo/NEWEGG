using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemCategorytemp")]
    public class ItemCategorytemp
    {
        public enum FromSystemStatus
        {
            PM = 0,
            Vender = 1
        }
        public ItemCategorytemp()
        {
            UpdateDate = DateTime.Now;
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "itemtempID，賣場暫存ID為必填欄位")]
        public int itemtempID { get; set; }
        public int ItemID { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "CategoryID，類別ID為必填欄位")]
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "FromSystem，系統別為必填欄位")]
        public string FromSystem { get; set; }
        [Required(ErrorMessage = "CreateUser，建立者為必填欄位")]
        public string CreateUser { get; set; }
        [Required(ErrorMessage = "CreateDate，建立者為必填欄位")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
