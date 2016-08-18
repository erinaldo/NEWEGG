using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Advertising
{
    public class AdvEvent
    {

        /// <summary>
        /// 流水號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Hash Code
        /// </summary>
        public string HashCode { get; set; }

        /// <summary>
        /// 點擊次數
        /// </summary>
        public int ClickNumber { get; set; }

        /// <summary>
        /// 廣告型態
        /// </summary>
        public Nullable<int> AdvType { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public Nullable<System.DateTime> StartDate { get; set; }

        /// <summary>
        /// 結束日期
        /// </summary>
        public Nullable<System.DateTime> EndDate { get; set; }

        /// <summary>
        /// 刪除日期
        /// </summary>
        public Nullable<System.DateTime> DelDate { get; set; }

        /// <summary>
        /// CSS class 1 名稱
        /// </summary>
        public string StyleClassName1 { get; set; }

        /// <summary>
        /// CSS class 2 名稱
        /// </summary>
        public string StyleClassName2 { get; set; }

        /// <summary>
        /// 完售圖片遮罩Css class名稱
        /// </summary>
        public string SoldoutClassName { get; set; }

        /// <summary>
        /// 圖片遮罩Css class名稱1
        /// </summary>
        public string ImgFilterClassName1 { get; set; }

        /// <summary>
        /// 圖片遮罩Css class名稱2
        /// </summary>
        public string ImgFilterClassName2 { get; set; }

        /// <summary>
        /// 起始前標題
        /// </summary>
        public string BeforeTitle { get; set; }

        /// <summary>
        /// 起始前標語
        /// </summary>
        public string BeforeSlogan { get; set; }

        /// <summary>
        /// 起始前連結位置
        /// </summary>
        public string BeforeLinkUrl { get; set; }

        /// <summary>
        /// 起始前圖片位置
        /// </summary>
        public string BeforeImgUrl { get; set; }

        /// <summary>
        /// 起始前圖片說明
        /// </summary>
        public string BeforeImgAlt { get; set; }

        /// <summary>
        /// 進行中標題
        /// </summary>
        public string StartTitle { get; set; }

        /// <summary>
        /// 進行中標語
        /// </summary>
        public string StartSlogan { get; set; }

        /// <summary>
        /// 進行中連結位置
        /// </summary>
        public string StartLinkUrl { get; set; }

        /// <summary>
        /// 進行中圖片位置
        /// </summary>
        public string StartImgUrl { get; set; }

        /// <summary>
        /// 進行中圖片說明
        /// </summary>
        public string StartImgAlt { get; set; }

        /// <summary>
        /// 結束後標題
        /// </summary>
        public string EndTitle { get; set; }

        /// <summary>
        /// 結束後標語
        /// </summary>
        public string EndSlogan { get; set; }

        /// <summary>
        /// 結束後連結位置
        /// </summary>
        public string EndLinkUrl { get; set; }

        /// <summary>
        /// 結束後圖片位置
        /// </summary>
        public string EndImgUrl { get; set; }

        /// <summary>
        /// 結束後圖片說明
        /// </summary>
        public string EndImgAlt { get; set; }

        /// <summary>
        /// 本廣告ItemID
        /// </summary>
        public Nullable<int> ItemID { get; set; }

        /// <summary>
        /// 推薦ItemIDs，逗號分隔
        /// </summary>
        public string RecommendItemIDs { get; set; }

        /// <summary>
        /// Api1名稱
        /// </summary>
        public string ExtraApi1 { get; set; }

        /// <summary>
        /// Api1呼叫方式(REST/Get/Post...)
        /// </summary>
        public string ExtraApiAction1 { get; set; }

        /// <summary>
        /// Api1參數(Uri格式)
        /// </summary>
        public string ExtraApiParameters1 { get; set; }

        /// <summary>
        /// Api2名稱
        /// </summary>
        public string ExtraApi2 { get; set; }

        /// <summary>
        /// Api2呼叫方式(REST/Get/Post...)
        /// </summary>
        public string ExtraApiAction2 { get; set; }

        /// <summary>
        /// Api2參數(Uri格式)
        /// </summary>
        public string ExtraApiParameters2 { get; set; }

        /// <summary>
        /// Api3名稱
        /// </summary>
        public string ExtraApi3 { get; set; }

        /// <summary>
        /// Api3呼叫方式(REST/Get/Post...)
        /// </summary>
        public string ExtraApiAction3 { get; set; }

        /// <summary>
        /// Api3參數(Uri格式)
        /// </summary>
        public string ExtraApiParameters3 { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 建立使用者
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 更新次數
        /// </summary>
        public int Updated { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }

        /// <summary>
        /// 更新使用者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 廣告區塊ID
        /// </summary>
        public Nullable<int> AdvEventTypeId { get; set; }

        /// <summary>
        /// 說明
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 排列順序, 數字愈小, 排列愈前面
        /// </summary>
        public int ShowOrder { get; set; }

        /// <summary>
        /// 上線狀態, 1:上線, 0:下線
        /// </summary>
        public int OnlineStatus { get; set; }
    }
}
