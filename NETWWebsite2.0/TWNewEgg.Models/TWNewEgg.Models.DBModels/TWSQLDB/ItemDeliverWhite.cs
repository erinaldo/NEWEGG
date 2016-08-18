using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("ItemDeliverWhite")]
    public class ItemDeliverWhite
    {
        public ItemDeliverWhite()
        {
            this.IsEnable = 0;
        }

        public enum type
        {
            Delivery = 0,
            StorePickUP = 1
        }

        public enum EnableStatus
        {
            啟用 = 0,
            關閉 = 1
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int ItemID { get; set; }

        public int DeliverType { get; set; }

        public int DeliverCode { get; set; }

        public int PayTypeID { get; set; }
        // 是否啟用
        public int IsEnable { get; set; }

        public string CreateUser { get; set; }

        public Nullable<DateTime> CreateDate { get; set; }

        public Nullable<int> Updated { get; set; }

        public string UpdateUser { get; set; }

        public Nullable<DateTime> UpdateDate { get; set; }
    }
}
