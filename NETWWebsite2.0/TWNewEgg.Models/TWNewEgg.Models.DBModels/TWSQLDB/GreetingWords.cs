using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{

    /// <summary>
    /// -----------------------add by bruce 20160329
    ///1 登入問候語
    ///2 節日問候卡
    /// </summary>
    [Table("GreetingWords")]
    public class GreetingWordsDB //: BaseDB
    {
        /// <summary>
        /// 系統流水編號
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [DisplayName("系統流水編號")]
        [Column("ID")]
        public int ID { get; set; }

       
      

        /// <summary>
        /// 文字描述
        /// </summary>
        [Required]
        [DisplayName("文字描述")]
        [Description("輸入問候的文字內容,如:端午節佳節快樂")]
        [MaxLength(27, ErrorMessage = "文字描述,請勿輸入超過27個字元")]
        [Column("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 顯示順序
        /// </summary>
        [DisplayName("顯示順序")]
        [Column("Showorder")]
        public int Showorder { get; set; }

        /// <summary>
        /// 是否顯示, 1:顯示, 0:不顯示
        /// </summary>
        [DisplayName("是否顯示, 1:顯示, 0:不顯示")]
        [Column("ShowAll")]
        public int ShowAll { get; set; }

        /// <summary>
        /// 點選連結
        /// </summary>
        [DisplayName("點選連結")]
        [Column("Clickpath")]
        public string Clickpath { get; set; }

        /// <summary>
        /// 分類Id
        /// -----------------------
        ///0 首頁熱門關鍵字
        ///1 登入問候語
        ///2 節日問候卡
        /// </summary>
        [DisplayName("分類, 1 登入問候語 | 2 節日問候卡")]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        [DisplayName("開始時間")]
        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        [DisplayName("結束時間")]
        [Column("EndDate")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 創建者
        /// </summary>
        [DisplayName("創建者")]
        [Column("CreateUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed)]
        [DisplayName("創建日期")]
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        [DisplayName("最後修改者")]
        [Column("UpdateUser")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        [DisplayName("最後修改日期")]
        [Column("UpdateDate")]
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 自訂代碼文字
        /// -----------------------
        /// use for 登入問候語
        /// </summary>
        [DisplayName("自訂代碼文字")]
        [Description("輸入自訂代碼文字,如:ABC123")]
        [MaxLength(27, ErrorMessage = "自訂代碼文字,請勿輸入超過27個字元")]
        [Column("CodeText")]
        public string CodeText { get; set; }

        /// <summary>
        /// 圖片位置
        /// -----------------------
        ///use for 節日問候卡
        /// </summary>
        [DisplayName("圖片位置")]
        [Column("ImageUrl")]
        public string ImageUrl { get; set; }

        


    }
}
