using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class DemainInsertSalesOrdersBySellerOutput
    {
        public DemainInsertSalesOrdersBySellerOutput()
        {
            this.salesorderitem_apportionedamount = 0m;
        }

        //----------public partial class salesorder----------//
        public string salesorder_code { get; set; }
        public Nullable<int> salesorder_salesordergroupid { get; set; }
        public string salesorder_idno { get; set; }
        public string salesorder_name { get; set; }
        public int salesorder_accountid { get; set; }
        public string salesorder_telday { get; set; }
        public string salesorder_telnight { get; set; }
        public string salesorder_mobile { get; set; }
        public string salesorder_email { get; set; }
        public Nullable<int> salesorder_paytypeid { get; set; }
        public Nullable<int> salesorder_paytype { get; set; }
        public Nullable<System.DateTime> salesorder_starvldate { get; set; }
        public string salesorder_cardholder { get; set; }
        public string salesorder_cardtelday { get; set; }
        public string salesorder_cardtelnight { get; set; }
        public string salesorder_cardmobile { get; set; }
        public string salesorder_cardloc { get; set; }
        public string salesorder_cardzip { get; set; }
        public string salesorder_cardaddr { get; set; }
        public string salesorder_cardno { get; set; }
        public string salesorder_cardnochk { get; set; }
        public string salesorder_cardtype { get; set; }
        public string salesorder_cardbank { get; set; }
        public string salesorder_cardexpire { get; set; }
        public Nullable<System.DateTime> salesorder_cardbirthday { get; set; }
        public string salesorder_invoreceiver { get; set; }
        public string salesorder_invoid { get; set; }
        public string salesorder_invotitle { get; set; }
        public string salesorder_involoc { get; set; }
        public string salesorder_invozip { get; set; }
        public string salesorder_invoaddr { get; set; }
        public string salesorder_recvname { get; set; }
        public string salesorder_recvengname { get; set; }
        public string salesorder_recvtelday { get; set; }
        public string salesorder_recvtelnight { get; set; }
        public string salesorder_recvmobile { get; set; }
        public Nullable<int> salesorder_delivtype { get; set; }
        public string salesorder_delivdata { get; set; }
        public string salesorder_delivloc { get; set; }
        public string salesorder_delivzip { get; set; }
        public string salesorder_delivaddr { get; set; }
        public string salesorder_delivengaddr { get; set; }
        public string salesorder_delivhitnote { get; set; }
        public Nullable<System.DateTime> salesorder_confirmdate { get; set; }
        public string salesorder_confirmnote { get; set; }
        public Nullable<System.DateTime> salesorder_authdate { get; set; }
        public string salesorder_authcode { get; set; }
        public string salesorder_authnote { get; set; }
        public Nullable<int> salesorder_hptype { get; set; }
        public Nullable<System.DateTime> salesorder_rcptdate { get; set; }
        public string salesorder_rcptnote { get; set; }
        public Nullable<System.DateTime> salesorder_expire { get; set; }
        public Nullable<System.DateTime> salesorder_datedel { get; set; }
        public string salesorder_coservername { get; set; }
        public string salesorder_servername { get; set; }
        public string salesorder_actcode { get; set; }
        public Nullable<byte> salesorder_status { get; set; }
        public string salesorder_statusnote { get; set; }
        public string salesorder_remoteip { get; set; }
        public Nullable<System.DateTime> salesorder_date { get; set; }
        public string salesorder_note { get; set; }
        public string salesorder_note2 { get; set; }
        public string salesorder_createuser { get; set; }
        public System.DateTime salesorder_createdate { get; set; }
        public Nullable<int> salesorder_updated { get; set; }
        public string salesorder_updateuser { get; set; }
        public Nullable<System.DateTime> salesorder_updatedate { get; set; }

        //----------public partial class salesorderitem----------//
        public string salesorderitem_code { get; set; }
        public string salesorderitem_salesordercode { get; set; }
        public int salesorderitem_itemid { get; set; }
        public int salesorderitem_itemlistid { get; set; }
        public int salesorderitem_productid { get; set; }
        public int salesorderitem_productlistid { get; set; }
        public string salesorderitem_name { get; set; }
        public decimal salesorderitem_price { get; set; }
        public decimal salesorderitem_displayprice { get; set; }    // 顯示的單一價格
        public decimal salesorderitem_discountprice { get; set; }   // 折扣金額
        public decimal salesorderitem_shippingexpense { get; set; } // 各別運費
        public decimal salesorderitem_serviceexpense { get; set; }  // 各別服務費
        public decimal salesorderitem_tax { get; set; }             // 各別稅費
        public decimal salesorderitem_itempricesum { get; set; }    // salesorderitems_price + salesorderitems_tax + salesorderitems_shippingexpense + salesorderitems_serviceexpense
        // 子單利息
        public decimal salesorderitem_installmentfee { get; set; }
        // 利息總額
        public decimal insRateFees { get; set; }
        //public Nullable<int> salesorderitem_warehouseid { get; set; }  //2013.12.20 add column by Bill
        public Nullable<decimal> salesorderitem_priceinst { get; set; }
        public int salesorderitem_qty { get; set; }
        public Nullable<decimal> salesorderitem_pricecoupon { get; set; }
        public string salesorderitem_coupons { get; set; }
        public Nullable<int> salesorderitem_redmtkout { get; set; }
        public Nullable<int> salesorderitem_redmbln { get; set; }
        public Nullable<int> salesorderitem_redmfdbck { get; set; }
        public Nullable<int> salesorderitem_status { get; set; }
        public string salesorderitem_statusnote { get; set; }
        public Nullable<System.DateTime> salesorderitem_date { get; set; }
        public string salesorderitem_attribs { get; set; }
        public string salesorderitem_note { get; set; }
        public Nullable<int> salesorderitem_wftkout { get; set; }
        public Nullable<int> salesorderitem_wfbln { get; set; }
        public Nullable<int> salesorderitem_adjprice { get; set; }
        public string salesorderitem_actid { get; set; }
        public Nullable<int> salesorderitem_acttkout { get; set; }
        public string salesorderitem_isnew { get; set; }
        public decimal salesorderitem_apportionedamount { get; set; } // 滿額贈折價金額
        public Nullable<int> salesorderitem_prodcutcostid { get; set; }
        public string salesorderitem_createuser { get; set; }
        public System.DateTime salesorderitem_createdate { get; set; }
        public Nullable<int> salesorderitem_updated { get; set; }
        public Nullable<System.DateTime> salesorderitem_updatedate { get; set; }
        public string salesorderitem_updateuser { get; set; }

        //----------public partial class salesorderitemext----------//
        public int salesorderitemext_id { get; set; }
        public string salesorderitemext_salesorderitemcode { get; set; }
        public string salesorderitemext_psproductid { get; set; }
        public string salesorderitemext_psmproductid { get; set; }
        public Nullable<int> salesorderitemext_psoriprice { get; set; }
        public string salesorderitemext_pssellcatid { get; set; }
        public string salesorderitemext_psattribname { get; set; }
        public string salesorderitemext_psmodelno { get; set; }
        public Nullable<int> salesorderitemext_pscost { get; set; }
        public Nullable<int> salesorderitemext_psfvf { get; set; }
    }
}
