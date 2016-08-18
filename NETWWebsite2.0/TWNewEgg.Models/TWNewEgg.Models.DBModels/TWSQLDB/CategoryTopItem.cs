using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("CategoryTopItem")]
    public class CategoryTopItem
    {
        [Key, Column("CategoryID", Order = 0)]
        public int CategoryID { get; set; }
        [Key, Column("ItemID", Order = 1)]
        public int ItemID { get; set; }
        /// <summary>
        /// 1：銷售TOP10
        /// 2：推薦商品
        /// </summary>
        public int ItemType { get; set; }
        /// <summary>
        /// 顯示排序，1開始
        /// </summary>
        public int Showorder { get; set; }
        /// <summary>
        /// 0：不顯示；1：顯示
        /// </summary>
        public int ShowAll { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
