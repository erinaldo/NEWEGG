using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("drawinglist")]
    public class DrawingList
    {
        public enum PrizeList : int 
        {
            尚未抽獎 = 0,
            銘謝惠顧 = 1,
            現金禮券 = 2,
            折價券 = 3,
            禮品 = 4,
            未中獎 = 5,
            獎品 = 6
        };
        public enum PrizeIsGive : int
        {
            未送出 = 0,
            送出 = 1,
            異常 = 2
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("流水號")]
        public int ID { get; set; }

        [DisplayName("抽獎者ID+時間組合序號(唯一)")]
        public string DrawingCode { get; set; }

        [DisplayName("抽獎者ID")]
        public int DrawingAccID { get; set; }

        [DisplayName("抽獎者抽獎次數")]
        public int DrawingTimes { get; set; }

        [DisplayName("抽獎者抽獎名稱")]
        public string DrawingName { get; set; }

        [DisplayName("抽獎者電話號碼")]
        public string DrawingPhone { get; set; }

        [DisplayName("抽獎者生日日期")]
        public DateTime DrawingBirthday { get; set; }

        [DisplayName("抽獎者性別")]
        public string DrawingGender { get; set; }

        [DisplayName("抽獎者地址")]
        public string DrawingAddress { get; set; }

        [DisplayName("抽獎者IDNO")]
        public string DrawingIDNO { get; set; }

        [DisplayName("獎品序號")]
        public int DrawingPrizeID { get; set; }

        [DisplayName("獎品種類")]
        public string DrawingPrizeName { get; set; }

        [DisplayName("獎品名稱")]
        public string DrawingPrizeItem { get; set; }

        [DisplayName("獎品金額")]
        public decimal DrawingPrizePrice { get; set; }

        [DisplayName("獎品送出狀態")]
        public int DrawingPrizeIsGive { get; set; }

        
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }

    }
}
