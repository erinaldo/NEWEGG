using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class InsertSalesOrdersBySellerInput
    {

        public InsertSalesOrdersBySellerInput()
        {
            this.instype = 0;
            this.item_id = "";
            //this.item_id = "'10'";
            this.salesorderPrefix = "'LBO'";
            this.salesorderitemPrefix = "'LBS'";
            this.pricesum = 0;
            this.ordernum = 1;
            this.note = "''";
            this.item_attribid = "";
            //this.item_attribid = "'1'";
            this.salesorder_telday = "''";
            this.salesorder_invoreceiver = "''";
            this.salesorder_invoid = "''";
            this.salesorder_invotitle = "''";
            this.salesorder_involoc = "''";
            this.salesorder_invozip = "''";
            this.salesorder_invoaddr = "''";
            this.salesorder_name = "''";
            this.salesorder_paytypeid = 0;
            this.salesorder_paytype = 1;
            this.salesorder_email = "''";
            this.salesorder_delivloc = "''";
            this.salesorder_delivzip = "''";
            this.salesorder_delivaddr = "''";
            this.salesorder_delivengaddr = "''";
            this.salesorder_idno = "''";
            this.salesorder_mobile = "''";
            this.salesorder_accountid = 0;
            this.salesorder_recvname = "''";
            this.salesorder_recvengname = "''";
            this.salesorder_recvmobile = "''";
            this.salesorder_recvtelday = "''";
            this.salesorder_cardno = "''";
            this.salesorder_cardtype = "''";
            this.salesorder_cardbank = "''";
            this.salesorder_cardexpire = "''";
            this.salesorder_cardbirthday = Convert.ToDateTime("1990/01/01");
            this.salesorder_cardloc = "''";
            this.salesorder_cardzip = "''";
            this.salesorder_cardaddr = "''";
            this.salesorder_status = 99; // 暫用99
            this.salesorders_note = "";
            //this.salesorders_note = "'主單註記'";
            this.salesorders_delivtype = "";
            //this.salesorders_delivtype = "'1'";
            this.salesorders_delivdata = "";
            //this.salesorders_delivdata = "''";
            this.salesorder_remoteip = "'127.0.0.1'";
            this.salesorder_coservername = "'newegg.com.tw'";
            this.salesorder_servername = "'web01'";
            this.salesorder_authcode = "''";
            this.salesorder_authdate = Convert.ToDateTime(DateTime.Now.ToShortDateString().ToString());
            this.salesorder_authnote = "''";
            this.salesorder_updateuser = "''";
            this.salesorders_itemname = "";
            //this.salesorders_itemname = "'SSD 128G'";
            this.salesorderitems_itemlistid = "";
            //this.salesorderitems_itemlistid = "'0'";
            this.salesorderitems_qty = "";
            //this.salesorderitems_qty = "'1'";
            this.salesorderitems_note = "";
            //this.salesorderitems_note = "'子單註記'";
            this.salesorderitems_price = "";
            this.salesorderitems_displayprice = "";
            this.salesorderitems_shippingexpense = ""; // 個別item的運費
            this.salesorderitems_serviceexpense = "";
            this.salesorderitems_tax = "";
            this.salesorderitems_itempricesum = "";
            //this.salesorderitems_price = "'5000'";
            this.salesorderitems_priceinst = "";
            this.salesorderitems_pricecoupon = "";
            this.salesorderitems_installmentfee = "";
            this.insRateFees = 0m;
            //this.salesorderitems_coupons = "";
            this.salesorderitems_redmbln = "";
            this.salesorderitems_redmtkout = "";
            this.salesorderitems_redmfdbck = "";
            this.salesorderitems_wfbln = "";
            this.salesorderitems_wftkout = "";
            this.salesorderitems_actid = "";
            this.salesorderitems_acttkout = "";
            this.salesorderitems_isnew = "";
            //this.salesorderitems_priceinst = "'1'";
            //this.salesorderitems_pricecoupon = "'2'";
            //this.salesorderitems_redmbln = "'3'";
            //this.salesorderitems_redmtkout = "'4'";
            //this.salesorderitems_redmfdbck = "'5'";
            //this.salesorderitems_wfbln = "'6'";
            //this.salesorderitems_wftkout = "'7'";
            //this.salesorderitems_actid = "'8'";
            //this.salesorderitems_acttkout = "'9'";
            this.itemlist_attribid = "";  // 0:一般 10:屬性 20:贈品
            //this.itemlist_attribid = "'10'";
            this.salesordergroupext_pscartid = 0;
            this.salesordergroupext_pssellerid = "''";
            this.salesordergroupext_pscarrynote = "''";
            this.salesordergroupext_pshasact = 0;
            this.salesordergroupext_pshaspartialauth = 0;
            this.salesorderitemexts_psproductid = "";
            this.salesorderitemexts_psmproductid = "";
            this.salesorderitemexts_psoriprice = "";
            this.salesorderitemexts_pssellcatid = "";
            this.salesorderitemexts_psattribname = "";
            this.salesorderitemexts_psmodelno = "";
            this.salesorderitemexts_pscost = "";
            this.salesorderitemexts_psfvf = "";
            this.salesorderitemexts_psproducttype = "";
            //this.salesorderitemexts_psproductid = "''";
            //this.salesorderitemexts_psmproductid = "''";
            //this.salesorderitemexts_psoriprice = "''";
            //this.salesorderitemexts_pssellcatid = "''";
            //this.salesorderitemexts_psattribname = "''";
            //this.salesorderitemexts_psmodelno = "''";
            //this.salesorderitemexts_pscost = "''";
            //this.salesorderitemexts_psfvf = "''";
            //this.salesorderitemexts_psproducttype = "''";
            this.ArrivalTime = ""; // 到達時間
            this.servicefees = 0; // 服務費
            this.PayTypeName = ""; // 銀行名稱與付款方式
            this.teldayzip = ""; // 訂購人市話區碼
            this.teldaynumber = ""; // 訂購人市話號碼
            this.teldayext = ""; // 訂購人市話分機
            this.recvteldayzip = ""; // 收件人市話區碼
            this.recvteldaynumber = ""; // 收件人市話號碼
            this.recvteldayext = ""; // 收件人市話分機
            this.cardCity = "";
            this.delivCity = "";
            this.memberUpdate = true;// 是否更新訂購人資訊(預設為ture執行更新動作)
            this.memberRecords = false;  // 是否將資料儲存於訂購人紀錄本
            this.delivRecords = false; // 是否將資料儲存於收件人紀錄本
            this.invoRecords = false; // 是否將資料儲存於統編紀錄本
            this.DeliverCode = 0;
        }

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
}