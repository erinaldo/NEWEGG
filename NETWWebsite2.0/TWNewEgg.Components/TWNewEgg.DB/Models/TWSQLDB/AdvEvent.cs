using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("advevent")]
    public class AdvEvent
    {

        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("流水號")]
        public int ID { get; set; }

        /// <summary>
        /// Hash Code
        /// </summary>
        [DisplayName("Hash Code")]
        public string HashCode { get; set; }

        /// <summary>
        /// 點擊次數
        /// </summary>
        [DisplayName("點擊次數")]
        public int ClickNumber { get; set; }

        /// <summary>
        /// 廣告型態
        /// </summary>
        [DisplayName("廣告型態")]
        public Nullable<int> AdvType { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        [DisplayName("起始日期")]
        public Nullable<System.DateTime> StartDate { get; set; }

        /// <summary>
        /// 結束日期
        /// </summary>
        [DisplayName("結束日期")]
        public Nullable<System.DateTime> EndDate { get; set; }

        /// <summary>
        /// 刪除日期
        /// </summary>
        [DisplayName("刪除日期")]
        public Nullable<System.DateTime> DelDate { get; set; }

        /// <summary>
        /// CSS class 1 名稱
        /// </summary>
        [DisplayName("CSS class 1 名稱")]
        public string StyleClassName1 { get; set; }

        /// <summary>
        /// CSS class 2 名稱
        /// </summary>
        [DisplayName("CSS class 2 名稱")]
        public string StyleClassName2 { get; set; }

        /// <summary>
        /// 完售圖片遮罩Css class名稱
        /// </summary>
        [DisplayName("完售圖片遮罩Css class名稱")]
        public string SoldoutClassName { get; set; }

        /// <summary>
        /// 圖片遮罩Css class名稱1
        /// </summary>
        [DisplayName("圖片遮罩Css class名稱1")]
        public string ImgFilterClassName1 { get; set; }

        /// <summary>
        /// 圖片遮罩Css class名稱2
        /// </summary>
        [DisplayName("圖片遮罩Css class名稱2")]
        public string ImgFilterClassName2 { get; set; }

        /// <summary>
        /// 起始前標題
        /// </summary>
        [DisplayName("起始前標題")]
        public string BeforeTitle { get; set; }

        /// <summary>
        /// 起始前標語
        /// </summary>
        [DisplayName("起始前標語")]
        public string BeforeSlogan { get; set; }

        /// <summary>
        /// 起始前連結位置
        /// </summary>
        [DisplayName("起始前連結位置")]
        public string BeforeLinkUrl { get; set; }

        /// <summary>
        /// 起始前圖片位置
        /// </summary>
        [DisplayName("起始前圖片位置")]
        public string BeforeImgUrl { get; set; }

        /// <summary>
        /// 起始前圖片說明
        /// </summary>
        [DisplayName("起始前圖片說明")]
        public string BeforeImgAlt { get; set; }

        /// <summary>
        /// 進行中標題
        /// </summary>
        [DisplayName("進行中標題")]
        public string StartTitle { get; set; }

        /// <summary>
        /// 進行中標語
        /// </summary>
        [DisplayName("進行中標語")]
        public string StartSlogan { get; set; }

        /// <summary>
        /// 進行中連結位置
        /// </summary>
        [DisplayName("進行中連結位置")]
        public string StartLinkUrl { get; set; }

        /// <summary>
        /// 進行中圖片位置
        /// </summary>
        [DisplayName("進行中圖片位置")]
        public string StartImgUrl { get; set; }

        /// <summary>
        /// 進行中圖片說明
        /// </summary>
        [DisplayName("進行中圖片說明")]
        public string StartImgAlt { get; set; }

        /// <summary>
        /// 結束後標題
        /// </summary>
        [DisplayName("結束後標題")]
        public string EndTitle { get; set; }

        /// <summary>
        /// 結束後標語
        /// </summary>
        [DisplayName("結束後標語")]
        public string EndSlogan { get; set; }

        /// <summary>
        /// 結束後連結位置
        /// </summary>
        [DisplayName("結束後連結位置")]
        public string EndLinkUrl { get; set; }

        /// <summary>
        /// 結束後圖片位置
        /// </summary>
        [DisplayName("結束後圖片位置")]
        public string EndImgUrl { get; set; }

        /// <summary>
        /// 結束後圖片說明
        /// </summary>
        [DisplayName("結束後圖片說明")]
        public string EndImgAlt { get; set; }

        /// <summary>
        /// 本廣告ItemID
        /// </summary>
        [DisplayName("本廣告ItemID")]
        public Nullable<int> ItemID { get; set; }

        /// <summary>
        /// 推薦ItemIDs，逗號分隔
        /// </summary>
        [DisplayName("推薦ItemIDs，逗號分隔")]
        public string RecommendItemIDs { get; set; }

        /// <summary>
        /// Api1名稱
        /// </summary>
        [DisplayName("Api1名稱")]
        public string ExtraApi1 { get; set; }

        /// <summary>
        /// Api1呼叫方式(REST/Get/Post...)
        /// </summary>
        [DisplayName("Api1呼叫方式(REST/Get/Post...)")]
        public string ExtraApiAction1 { get; set; }

        /// <summary>
        /// Api1參數(Uri格式)
        /// </summary>
        [DisplayName("Api1參數(Uri格式)")]
        public string ExtraApiParameters1 { get; set; }

        /// <summary>
        /// Api2名稱
        /// </summary>
        [DisplayName("Api2名稱")]
        public string ExtraApi2 { get; set; }

        /// <summary>
        /// Api2呼叫方式(REST/Get/Post...)
        /// </summary>
        [DisplayName("Api2呼叫方式(REST/Get/Post...)")]
        public string ExtraApiAction2 { get; set; }

        /// <summary>
        /// Api2參數(Uri格式)
        /// </summary>
        [DisplayName("Api2參數(Uri格式)")]
        public string ExtraApiParameters2 { get; set; }

        /// <summary>
        /// Api3名稱
        /// </summary>
        [DisplayName("Api3名稱")]
        public string ExtraApi3 { get; set; }

        /// <summary>
        /// Api3呼叫方式(REST/Get/Post...)
        /// </summary>
        [DisplayName("Api3呼叫方式(REST/Get/Post...)")]
        public string ExtraApiAction3 { get; set; }

        /// <summary>
        /// Api3參數(Uri格式)
        /// </summary>
        [DisplayName("Api3參數(Uri格式)")]
        public string ExtraApiParameters3 { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [DisplayName("建立時間")]
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 建立使用者
        /// </summary>
        [DisplayName("建立使用者")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 更新次數
        /// </summary>
        [DisplayName("更新次數")]
        public int Updated { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        [DisplayName("更新時間")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

        /// <summary>
        /// 更新使用者
        /// </summary>
        [DisplayName("更新使用者")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 廣告區塊ID
        /// </summary>
        [DisplayName("廣告區塊ID")]
        public Nullable<int> AdvEventTypeId { get; set; }

        /// <summary>
        /// 說明
        /// </summary>
        [DisplayName("說明")]
        public string Memo { get; set; }

        /// <summary>
        /// 排列順序, 數字愈小, 排列愈前面
        /// </summary>
        [DisplayName("排列順序")]
        public int ShowOrder { get; set; }

        /// <summary>
        /// 上線狀態, 1:上線, 0:下線
        /// </summary>
        [DisplayName("上線狀態")]
        public int OnlineStatus { get; set; }
    }
}
