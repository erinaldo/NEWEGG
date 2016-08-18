using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("edmbooklist")]
    public class EDMBookList
    {
        public EDMBookList()
        {
            //this.AdvTypeCode = 0; // AdvEventType Table 中的 AdvTypeCode
            this.EDMTypeName = "EDMBookList"; // EDM顯示名稱
            //this.EDMOrder = 0; // EDM排列順序
            //this.AdvEventID = 0; // AdvEvent Table的ID
            this.RowSpan = 1; // 橫向所佔欄位
            this.ColSpan = 1; // 直向所佔欄位
            this.FirstStyle = "";
            this.SecondStyle = "";
            this.ThirdStyle = "";
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int EDMBookID { get; set; } // EDMBook Table 中的 ID
        public string EDMTypeName { get; set; } // EDM顯示名稱
        public int EDMOrder { get; set; } // EDM排列順序
        public int AdvEventID { get; set; } // AdvEvent Table的ID
        public int RowSpan { get; set; } // 橫向所佔欄位
        public int ColSpan { get; set; } // 直向所佔欄位
        public string FirstStyle { get; set; } // 1stStyle設置
        public string SecondStyle { get; set; } // 2ndStyle設置
        public string ThirdStyle { get; set; } // 3rdStyle設置
        public Nullable<DateTime> StartDate { get; set; } // EDM起始時間
        public Nullable<DateTime> EndDate { get; set; } // EDM結束時間
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
