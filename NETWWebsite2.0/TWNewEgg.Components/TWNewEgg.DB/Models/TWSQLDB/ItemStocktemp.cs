using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemStocktemp")]
    public class ItemStocktemp
    {
        public ItemStocktemp()
        {
            UpdateDate = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "producttempID，商品暫存ID為必填欄位")]
        public int producttempID { get; set; }
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Qty，庫存數為必填欄位")]
        public int Qty { get; set; }
        public int QtyReg { get; set; }
        public int SafeQty { get; set; }
        public int Fdbcklmt { get; set; }
        [Required(ErrorMessage = "CreateUser，建立者為必填欄位")]
        public string CreateUser { get; set; }
        [Required(ErrorMessage = "CreateDate，建立者為必填欄位")]
        public DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
