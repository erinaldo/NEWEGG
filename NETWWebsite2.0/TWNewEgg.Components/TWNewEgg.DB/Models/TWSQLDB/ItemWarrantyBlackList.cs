using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemWarrantyBlackList")]
    public class ItemWarrantyBlackList
    {

        public ItemWarrantyBlackList()
        {
            Updated = 0;
            Update = DateTime.Now;
        }

        // 流水號
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //商品編號
        public int Itemid { get; set; }
        //商品原文說明
        public int Desc { get; set; }
        //商品規格說明
        public int ProductProperty { get; set; }
        //更新日期
        public Nullable<System.DateTime> Update { get; set; }
        //更新過
        public Nullable<int> Updated { get; set; }
        //更新者
        public string UpdateUser { get; set; }
    }
}