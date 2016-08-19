using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class PayTypeDeliverInfo
    {
        public PayTypeDeliverInfo()
        {
            this.PayType = 0;
            this.PayTypeID = 0;
            this.DeliverCode = 0;
            this.DeliverWay = new getDeliverType();
            this.Deliver = new getDeliver();
            this.ConvenienceStore = new getConvenienceStore();
        }

        // 金流PayTypeID
        public int PayTypeID { get; set; }
        // 賣家ID
        //public int SellerID { get; set; }
        // 商品配達方式
        public getDeliverType DeliverWay { get; set; }
        // 貨運公司編號
        public int DeliverCode { get; set; }
        // 金流類型 (1、3、6、10、12、18、24、103、106、110、112、118、124、30、31、32、33......)
        public int PayType { get; set; }
        // 店配
        public getConvenienceStore ConvenienceStore { get; set; }
        // 配達
        public getDeliver Deliver { get; set; }
    }

    /// <summary>
    /// 商品配達方式
    /// </summary>
    public class getDeliverType
    {
        public getDeliverType()
        {
            this.Delivery = true;
            this.PickupByStore = false;
        }
        // 宅配
        public bool Delivery { get; set; }
        // 店配
        public bool PickupByStore { get; set; }
    }

    public class getConvenienceStore
    {
        public getConvenienceStore()
        {
            this.SevenEleven = false;
            this.FamilyMart = false;
            this.HiLife = false;
            this.OKMart = false;
        }
        // 7-11便利商店
        public bool SevenEleven { get; set; }
        // 全家便利商店
        public bool FamilyMart { get; set; }
        // 萊爾富便利商店
        public bool HiLife { get; set; }
        // OK便利商店
        public bool OKMart { get; set; }
    }

    public class getDeliver
    {
        public getDeliver()
        {
            this.HCT = true;
        }
        // 新竹貨運
        public bool HCT { get; set; }
    }
}