using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Item
{
    /// <summary>
    /// 賣場頁所使用的ViewModel
    /// </summary>
    public class ItemBasic
    {

       


        /// <summary>
        /// 商品付款方式
        /// </summary>
        public enum ItemPaymentOption
        {
            信用卡一次付清 = 1,
            三期零利率 = 3,
            六期零利率 = 6,
            十期零利率 = 10,
            十二期零利率 = 12,
            十八期零利率 = 18,
            二十四期零利率 = 24,
            十期分期 = 110,
            十二期分期 = 112,
            十八期分期 = 118,
            二十四期分期 = 124,
            網路ATM = 30,
            貨到付款 = 31,
            超商付款 = 32,
            電匯 = 33,
            實體ATM = 34,
            歐付寶付款 = 501,
            信用卡紅利折抵 = 201
        }

        /// <summary>
        /// 商品配送方式
        /// </summary>
        public enum ItemDeliveryOption
        {
            宅配 = 0,
            超商取貨 = 1
        }

        public enum ItemSourceOption
        {
            台灣 = 1,
            美國 = 2,
            加拿大 = 3,
            中國 = 4,
            香港 = 5
        }

        #region 商品基本資訊
        /// <summary>
        /// Item Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Item Id
        /// </summary>
        public int ProuctId { get; set; }

        /// <summary>
        /// Item Category
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// Item品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item品名下的小標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否為美蛋商品備註(是間配1、美蛋商品)
        /// </summary>
        public bool IsUSRemark { get; set; }

        /// <summary>
        /// 商家ID
        /// </summary>
        public int SellerID { get; set; }

        /// <summary>
        /// 交易模式
        /// </summary>
        public int DelvType { get; set; }

        /// <summary>
        /// Item Slogan
        /// </summary>
        public string Slogan { get; set; }

        /// <summary>
        /// 原價
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 目前商品可銷售數量
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 產品圖Url
        /// </summary>
        public List<string> ImgUrlList { get; set; }

        /// <summary>
        /// 商品來源
        /// </summary>
        public ItemSourceOption ItemSource { get; set; }

        /// <summary>
        /// 預計到貨時間
        /// </summary>
        public string ArrivalTime { get; set; }

        /// <summary>
        /// 商品說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 商品原文說明
        /// </summary>
        public string SourceDescription { get; set; }

        /// <summary>
        /// 商品規格說明
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 保固/退換說 說明
        /// </summary>
        public string Warranty { get; set; }

        /// <summary>
        /// 網友心得
        /// </summary>
        public string UserReviews { get; set; }

        /// <summary>
        /// 賣場開始時間
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// 賣場結束時間
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// 商品DeliveryType, 如間配、直購品等等
        /// </summary>
        public int ItemDeliveryType { get; set; }

        /// <summary>
        /// 商品狀態
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 商品廠商型號
        /// </summary>
        public string SellerProductID { get; set; }

        /// <summary>
        /// 商品是否新品[Y: 新品, N:二手]
        /// </summary>
        public string IsNew { get; set; }

        /// <summary>
        /// 品牌故事
        /// </summary>
        public string BrandStory { get; set; }

        /// <summary>
        /// 是否任選館商品
        /// </summary>
        public int IsChooseAny { get; set; }

        /// <summary>
        /// 是否隱藏
        /// </summary>
        public int ShowOrder { get; set; }
        /// <summary>
        /// Discard4 
        /// Y:是廢四機
        /// N:廢四機
        /// </summary>
        public string Discard4 { get; set; }

        #endregion

        #region 物流金流
        /// <summary>
        /// 付款方式
        /// </summary>
        public List<ItemPaymentOption> PaymentType { get; set; }

        /// <summary>
        /// 配送到消費者手中的台灣物流方式
        /// </summary>
        public List<ItemDeliveryOption> DeliveryType { get; set; }
        #endregion

        #region 活動訊息
        /// <summary>
        /// 優惠價格
        /// </summary>
        public decimal PromotionPrice { get; set; }

        /// <summary>
        /// 活動訊息
        /// </summary>
        public string PromotionMessage { get; set; }

        /// <summary>
        /// 優惠活動名稱
        /// </summary>
        public List<TWNewEgg.Models.ViewModels.Promotion.PromotionDetail_View> PromotionBasicList { get; set; }

        /// <summary>
        /// 活動倒數計時
        /// </summary>
        public Nullable<DateTime> Countdown { get; set; }
        #endregion

        #region 同類型產品列表

        /// <summary>
        /// 群組品項
        /// </summary>
        public Dictionary<int,List<ItemMarketGroup>> DictItemMarketGroup { get; set; }
        #endregion
        /// <summary>
        /// 利用 item id 查詢對應的信用卡分期付款方式
        /// </summary>
        public List<ItemPayType> listItemPayType { get; set; }
    }

    public class ItemApi
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
        public ItemBasic Body { get; set; }
        
    }
    #region 利用 item id 查詢對應的信用卡分期付款方式
    public class ItemPayType
    {
        public int PayType0rateNum { get; set; }
        //顯示在前台的可用銀行
        public string PayTypeBankStrForList { get; set; }
        //期數
        public int Staging { get; set; }
        //利息
        public string InsRate { get; set; }
        //可使用的銀行數量
        public int canUseBankCount { get; set; }
    }
    
    #endregion
}
