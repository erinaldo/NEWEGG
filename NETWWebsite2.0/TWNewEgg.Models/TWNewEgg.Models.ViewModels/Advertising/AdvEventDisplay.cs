﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Advertising
{
    public class AdvEventDisplay
    {

        /// <summary>
        /// Hash Code
        /// </summary>
        public string HC { get; set; }

        /// <summary>
        /// 點擊次數
        /// </summary>
        public int CN { get; set; }

        /// <summary>
        /// 廣告型態
        /// </summary>
        public string AT { get; set; }

        /// <summary>
        /// 起始日期
        /// </summary>
        public Nullable<System.DateTime> SD { get; set; }

        /// <summary>
        /// 結束日期
        /// </summary>
        public Nullable<System.DateTime> ED { get; set; }

        /// <summary>
        /// CSS class名稱
        /// </summary>
        public string SCN1 { get; set; }

        /// <summary>
        /// CSS class名稱
        /// </summary>
        public string SCN2 { get; set; }

        /// <summary>
        /// 完售圖片遮罩Css class名稱
        /// </summary>
        public string SoldOut { get; set; }

        /// <summary>
        /// 圖片遮罩Css class名稱1
        /// </summary>
        public string IFC1 { get; set; }

        /// <summary>
        /// 圖片遮罩Css class名稱2
        /// </summary>
        public string IFC2 { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 標語
        /// </summary>
        public string Slogan { get; set; }

        /// <summary>
        /// 連結位置
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 圖片位置
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 圖片說明
        /// </summary>
        public string ImgAlt { get; set; }

        /// <summary>
        /// 本廣告ItemID
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 本廣告ItemID Pic起始編號
        /// </summary>
        public int ItemIDPicStart { get; set; }

        /// <summary>
        /// 本廣告ItemID Pic結束編號
        /// </summary>
        public int ItemIDPicEnd { get; set; }

        /// <summary>
        /// 本廣告ItemID數量
        /// </summary>
        public int ItemStock { get; set; }

        /// <summary>
        /// 本廣告ItemID model
        /// </summary>
        public string ItemModel { get; set; }

        /// <summary>
        /// 本廣告ItemID manufacture
        /// </summary>
        public string ItemManufacture { get; set; }

        /// <summary>
        /// 本廣告ItemID市場價格
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 本廣告ItemID販售價格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 本廣告ItemID運輸價格
        /// </summary>
        public decimal ShippingPrice { get; set; }

        /// <summary>
        /// 本廣告ItemID稅負價格
        /// </summary>
        public decimal TaxPrice { get; set; }

        /// <summary>
        /// 本廣告ItemID服務費價格
        /// </summary>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// 本廣告ItemID折扣
        /// </summary>
        public string SavingPercent { get; set; }

        /// <summary>
        /// 本廣告ItemID category id
        /// </summary>
        public string CategoryID { get; set; }

        /// <summary>
        /// 本廣告ItemID category name
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 推薦ItemIDs，逗號分隔
        /// </summary>
        public string RItemIDs { get; set; }

        /// <summary>
        /// Api1名稱
        /// </summary>
        public string EA1 { get; set; }

        /// <summary>
        /// Api1呼叫方式(REST/Get/Post...)
        /// </summary>
        public string EAA1 { get; set; }

        /// <summary>
        /// Api1參數(Uri格式)
        /// </summary>
        public string EAP1 { get; set; }

        /// <summary>
        /// Api2名稱
        /// </summary>
        public string EA2 { get; set; }

        /// <summary>
        /// Api2呼叫方式(REST/Get/Post...)
        /// </summary>
        public string EAA2 { get; set; }

        /// <summary>
        /// Api2參數(Uri格式)
        /// </summary>
        public string EAP2 { get; set; }

        /// <summary>
        /// Api3名稱
        /// </summary>
        public string EA3 { get; set; }

        /// <summary>
        /// Api3呼叫方式(REST/Get/Post...)
        /// </summary>
        public string EAA3 { get; set; }

        /// <summary>
        /// Api3參數(Uri格式)
        /// </summary>
        public string EAP3 { get; set; }

        public AdvEventDisplay() { }
        public AdvEventDisplay(AdvEvent aAdvEvent)
        {
            this.HC = aAdvEvent.HashCode;
            this.CN = aAdvEvent.ClickNumber;
            this.AT = ((AdvEventType.AdvTypeOption)aAdvEvent.AdvType).ToString();
            this.SD = aAdvEvent.StartDate;
            this.ED = aAdvEvent.EndDate;
            this.SCN1 = aAdvEvent.StyleClassName1;
            this.SCN2 = aAdvEvent.StyleClassName2;
            this.IFC1 = aAdvEvent.ImgFilterClassName1;
            this.IFC2 = aAdvEvent.ImgFilterClassName2;

            this.RItemIDs = aAdvEvent.RecommendItemIDs;
            this.EA1 = aAdvEvent.ExtraApi1;
            this.EAA1 = aAdvEvent.ExtraApiAction1;
            this.EAP1 = aAdvEvent.ExtraApiParameters1;
            this.EA2 = aAdvEvent.ExtraApi2;
            this.EAA2 = aAdvEvent.ExtraApiAction2;
            this.EAP2 = aAdvEvent.ExtraApiParameters2;
            this.EA3 = aAdvEvent.ExtraApi3;
            this.EAA3 = aAdvEvent.ExtraApiAction3;
            this.EAP3 = aAdvEvent.ExtraApiParameters3;
        }

        #region ADVEventDisplay function for Item
        public void ifBeforeClearItemInfo(bool clearMoney, bool clearMarketPrice)
        {
            this.HC = "";
            this.CN = 0;
            this.ItemID = 0;
            this.ItemIDPicStart = 0;
            this.ItemIDPicEnd = 0;
            this.ItemStock = 0;
            this.ItemModel = "";
            this.ItemManufacture = "";
            if (clearMoney)
            {
                this.SalePrice = 0;
                this.ShippingPrice = 0;
                this.TaxPrice = 0;
                this.ServiceFee = 0;
            }

            if (clearMarketPrice)
            {
                this.MarketPrice = 0;
                this.SavingPercent = "";
            }
            else
            {
                this.SavingPercent = "100";
            }


            this.CategoryID = "";
            this.CategoryName = "";

            this.RItemIDs = "";
            this.EA1 = "";
            this.EAA1 = "";
            this.EAP1 = "";
            this.EA2 = "";
            this.EAA2 = "";
            this.EAP2 = "";
            this.EA3 = "";
            this.EAA3 = "";
            this.EAP3 = "";
        }
        public void setItemLinkAndImgUrl(string itemName, string itemLink, string itemImg)
        {
            if ((this.Title == null || this.Title == "") && (itemName != null && itemName != ""))
            {
                this.Title = itemName;
            }
            if ((this.Slogan == null || this.Slogan == "") && (itemName != null && itemName != ""))
            {
                this.Slogan = itemName;
            }
            if ((this.LinkUrl == null || this.LinkUrl == "") && (itemLink != null && itemLink != ""))
            {
                this.LinkUrl = itemLink;
            }
            if ((this.ImgUrl == null || this.ImgUrl == "") && (itemImg != null && itemImg != ""))
            {
                this.ImgUrl = itemImg;
            }
            if ((this.ImgAlt == null || this.ImgAlt == "") && (itemName != null && itemName != ""))
            {
                this.ImgAlt = itemName;
            }
        }
        public void setBeforeInfoUsingRealDate(AdvEvent aAdvEvent)
        {
            if (aAdvEvent.BeforeTitle != null && aAdvEvent.BeforeTitle != "")
            {
                this.Title = aAdvEvent.BeforeTitle;
            }
            if (aAdvEvent.BeforeSlogan != null && aAdvEvent.BeforeSlogan != "")
            {
                this.Slogan = aAdvEvent.BeforeSlogan;
            }
            if (aAdvEvent.BeforeLinkUrl != null && aAdvEvent.BeforeLinkUrl != "")
            {
                this.LinkUrl = aAdvEvent.BeforeLinkUrl;
            }
            if (aAdvEvent.BeforeImgUrl != null && aAdvEvent.BeforeImgUrl != "")
            {
                this.ImgUrl = aAdvEvent.BeforeImgUrl;
                ifBeforeClearItemInfo(true, false);
            }
            if (aAdvEvent.BeforeImgAlt != null && aAdvEvent.BeforeImgAlt != "")
            {
                this.ImgAlt = aAdvEvent.BeforeImgAlt;
            }

        }
        #endregion

    }
}
