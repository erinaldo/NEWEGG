using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class DomainInsertSOGroupOrderItem
    {
        //--------insertSalesOrdersBySeller--------//
        public int instype { get; set; }                     // 新增的類型 0是預設 1表示Seller
        public string item_id { get; set; }                  // 主件的item_id
        public string salesorderPrefix { get; set; }
        public string salesorderitemPrefix { get; set; }
        public decimal pricesum { get; set; }
        public int ordernum { get; set; }
        public string note { get; set; }
        public string item_attribid { get; set; }            // 主件屬性id
        public string salesorder_telday { get; set; }        // 訂單白天聯絡電話
        public string salesorder_invoreceiver { get; set; }  // 發票接收狀態, 託管 or 寄送
        public string salesorder_invoid { get; set; }        // 發票統一編號
        public string salesorder_invotitle { get; set; }     // 發票公司名稱
        public string salesorder_involoc { get; set; }       // 發票區域
        public string salesorder_invozip { get; set; }       // 發票郵遞區號
        public string salesorder_invoaddr { get; set; }      // 發票寄送地址
        public string salesorder_name { get; set; }          // 訂購人姓名
        public int salesorder_paytypeid { get; set; }        // 訂單付款類型ID
        public int salesorder_paytype { get; set; }          // 訂單付款類型
        public string salesorder_email { get; set; }         // 訂單email
        public string salesorder_delivloc { get; set; }      // 配送區域
        public string salesorder_delivzip { get; set; }      // 配送郵遞區號
        public string salesorder_delivaddr { get; set; }     // 配送地址
        public string salesorder_delivengaddr { get; set; }  // 英文配送地址
        public string salesorder_idno { get; set; }
        public string salesorder_mobile { get; set; }
        public int salesorder_accountid { get; set; }
        public string salesorder_recvname { get; set; }
        public string salesorder_recvengname { get; set; }   // 英文名
        public string salesorder_recvmobile { get; set; }
        public string salesorder_recvtelday { get; set; }
        public string salesorder_cardno { get; set; }
        public string salesorder_cardtype { get; set; }
        public string salesorder_cardbank { get; set; }
        public string salesorder_cardexpire { get; set; }
        public DateTime salesorder_cardbirthday { get; set; }
        public string salesorder_cardloc { get; set; }
        public string salesorder_cardzip { get; set; }
        public string salesorder_cardaddr { get; set; }
        public int salesorder_status { get; set; }
        public string salesorders_note { get; set; }
        public string salesorders_delivtype { get; set; }
        public string salesorders_delivdata { get; set; }
        public string salesorder_remoteip { get; set; }
        public string salesorder_coservername { get; set; }
        public string salesorder_servername { get; set; }
        public string salesorder_authcode { get; set; }
        public DateTime salesorder_authdate { get; set; }
        public string salesorder_authnote { get; set; }
        public string salesorder_updateuser { get; set; }
        // public Int64 order_ordergroupid { get; set; }
        public string salesorders_itemname { get; set; }
        public string salesorderitems_itemlistid { get; set; }
        public string salesorderitems_qty { get; set; }
        public string salesorderitems_note { get; set; }
        public string salesorderitems_price { get; set; }
        public string salesorderitems_displayprice { get; set; }    // 顯示的單一價格
        public string salesorderitems_discountprice { get; set; }   // 折扣金額
        public string salesorderitems_shippingexpense { get; set; } // 各別運費
        public string salesorderitems_serviceexpense { get; set; }  // 各別服務費
        public string salesorderitems_tax { get; set; }             // 各別稅費
        public string salesorderitems_itempricesum { get; set; }    // salesorderitems_price + salesorderitems_tax + salesorderitems_shippingexpense + salesorderitems_serviceexpense
        // 子單利息
        public string salesorderitems_installmentfee { get; set; }
        // 利息總額
        public decimal insRateFees { get; set; }
        //public string salesorderitems_warehouseid { get; set; }  //2013.12.20 add column by Bill
        public string salesorderitems_priceinst { get; set; }
        public string salesorderitems_pricecoupon { get; set; }
        public string salesorderitems_coupons { get; set; }
        public string salesorderitems_redmbln { get; set; }
        public string salesorderitems_redmtkout { get; set; }
        public string salesorderitems_redmfdbck { get; set; }
        public string salesorderitems_wfbln { get; set; }
        public string salesorderitems_wftkout { get; set; }
        public string salesorderitems_actid { get; set; }
        public string salesorderitems_acttkout { get; set; }
        public string salesorderitems_isnew { get; set; }
        public string itemlist_attribid { get; set; }        //配件屬性id
        public int salesordergroupext_pscartid { get; set; }
        public string salesordergroupext_pssellerid { get; set; }
        public string salesordergroupext_pscarrynote { get; set; }
        public int salesordergroupext_pshasact { get; set; }
        public int salesordergroupext_pshaspartialauth { get; set; }
        public string salesorderitemexts_psproductid { get; set; }
        public string salesorderitemexts_psmproductid { get; set; }
        public string salesorderitemexts_psoriprice { get; set; }
        public string salesorderitemexts_pssellcatid { get; set; }
        public string salesorderitemexts_psattribname { get; set; }
        public string salesorderitemexts_psmodelno { get; set; }
        public string salesorderitemexts_pscost { get; set; }
        public string salesorderitemexts_psfvf { get; set; }
        public string salesorderitemexts_psproducttype { get; set; }
        public string dboutput { get; set; }

        public string buyingcartinfo { get; set; }
        public string auth_code_3 { get; set; }
        public string invore3_2 { get; set; }
        public string ArrivalTime { get; set; }
        public decimal servicefees { get; set; }
        public string PayTypeName { get; set; }

        public string teldayzip { get; set; }
        public string teldaynumber { get; set; }
        public string teldayext { get; set; }
        public string recvteldayzip { get; set; }
        public string recvteldaynumber { get; set; }
        public string recvteldayext { get; set; }
        public string cardCity { get; set; }
        public string cardaddr { get; set; }
        public string delivCity { get; set; }
        public string delivaddr { get; set; }
        // 更新訂購人資訊、新增收件人紀錄、新增統編紀錄
        public bool memberRecords { get; set; }
        public bool memberUpdate { get; set; }
        public bool delivRecords { get; set; }
        public bool invoRecords { get; set; }

        //Coupon區段
        public string CouponJsonString { get; set; }    //coupons:["ItemId:0001", "buySum:5"]
        public decimal TotalCouponValue { get; set; }
        public string CouponHiddenObject { get; set; }  //用於前端顯示Hidden物件的字串, 如"<input type='hidden' id='usedCoupon_51585' name='usedCoupon_51585' value=';251;' /><input type='hidden' id='usedCoupon_51604' name='usedCoupon_51604' value=';252;' />"
        //end of Coupon區段

        // 會員資訊 member
        public Nullable<int> member_sex { get; set; } // 訂購人稱謂
        public string member_firstname { get; set; } // 訂購人姓
        public string member_lastname { get; set; } // 訂購人名
        public string member_mobile1 { get; set; }
        public string member_mobile2 { get; set; }
        // end of 會員資訊 member
        public Nullable<int> member_recvsex { get; set; } // 收件人稱謂
        public string member_recvfirstname { get; set; } // 收件人姓
        public string member_recvlastname { get; set; } // 收件人名
        // 便利商店資訊
        public string BankName { get; set; }
        public string PickupWaySetting { get; set; } // 使用宅配or便利商店配送
        public string ConvenienceStoreName { get; set; } // 門市名稱 Stnm(取貨店名)
        public string storeRoute { get; set; } // DCRONO(路線路順)
        public string storeStno { get; set; } // Stno(取貨門市編號)
        public int DeliverCode { get; set; }

        // promotionGift list
        public string listJSONPromotion { get; set; } // 購物車內所有商品的PromotionGift清單
        public string listJSONPromotionOversea { get; set; }

        // Electric Invoice 
        public string invoiceCarType { get; set; } //載具型態
        public string invoiceCarCell { get; set; } //手機載具碼
        public string invoiceCarNatu { get; set; } //自然人憑證碼
        public string invoiceDonCode { get; set; } //捐贈愛心碼

        public string itemCategoey { get; set; } //前台紀錄Category
    }

    public enum SOType
    {
        SalesOrderGroup,
        SalesOrder,
        SalesOrderItem
        
    }

}
