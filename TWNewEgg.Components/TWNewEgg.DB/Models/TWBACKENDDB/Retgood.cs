using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("retgood")]
    public partial class Retgood
    {
        public enum status : short
        {
            退貨處理中 = 0,
            退貨中 = 1,
            完成退貨 = 2,

            退款中 = 3,
            完成退款 = 4,

            退貨異常 = 5,
            退款異常 = 6,

            退貨取消 = 7,
            退款取消 = 8,

            退款尚未批示 = 98,
            進入退款程序 = 99
        }

        public enum reason : short
        {
            規格不合 = 1,
            與想像不符 = 2,
            其他原因 = 3,
            客戶拒收 = 4,
            配達失敗 = 5,
            [Description("商品故障/瑕疵")] 商品故障瑕疵=6,
            [Description("商品寄錯/缺件")] 商品寄錯缺件=7,
            [Description("商品尺寸或規格與網頁標示不同或不夠完整")]商品尺寸或規格與網頁標示不同或不夠完整=8,
            [Description("品質不佳(質感、性能等)")]品質不佳質感性能等=9,
            [Description("商品不符所需或不適合")]商品不符所需或不適合=10,
            [Description("重複購買/不需要了")]重複購買不需要了=11,
            [Description("重訂(改買別款、改付款方式等")]重訂改買別款改付款方式等=12,
            [Description("價格比別家貴")]價格比別家貴=13,
            [Description("同商品降價或有更優惠的活動")]同商品降價或有更優惠的活動=14,
            [Description("其他")]其他=15,
            [Description("廠商通知缺貨")]廠商通知缺貨=16
        }

        public enum declinedornot : short
        {
            不為拒收或為配達的情況 = 0,
            拒收或為配達的情況 = 1,
        }

        public enum ProductStatuses
        {
            未開箱 = 0,
            已開箱 = 1,
            已損壞 = 2,
            錯品 = 3
        }
        /// <summary>
        /// 退貨單序號
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 退貨單號
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 供應商序號
        /// </summary>
        public Nullable<int> SupplierID { get; set; }
        /// <summary>
        /// 商品序號
        /// </summary>
        public Nullable<int> ProductID { get; set; }
        /// <summary>
        /// 立案方式
        /// </summary>
        public Nullable<int> CreatedFRM { get; set; }
        /// <summary>
        /// 登錄人
        /// </summary>
        public Nullable<int> RetgoodType { get; set; }
        /// <summary>
        /// 建檔日期
        /// </summary>
        public Nullable<System.DateTime> Date { get; set; }
        /// <summary>
        /// 退貨配達方式
        /// </summary>
        public string RetgoodUser { get; set; }
        /// <summary>
        /// 結案日期
        /// </summary>
        public Nullable<System.DateTime> FinalDate { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public Nullable<int> Status { get; set; }
        /// <summary>
        /// 單價
        /// </summary>
        public Nullable<decimal> Price { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public Nullable<int> Qty { get; set; }
        /// <summary>
        /// 收到商品日期
        /// </summary>
        public Nullable<System.DateTime> RECVGoodDate { get; set; }
        /// <summary>
        /// 收到發票日期
        /// </summary>
        public Nullable<System.DateTime> RecvinvoDate { get; set; }
        /// <summary>
        /// 發票處理
        /// </summary>
        public Nullable<int> InvocFmrslt { get; set; }
        /// <summary>
        /// 折讓單開立日期
        /// </summary>
        public Nullable<System.DateTime> InvocFMDate { get; set; }
        /// <summary>
        /// 發票處理人
        /// </summary>
        public string InvocFMUser { get; set; }
        /// <summary>
        /// 結案人
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// 退貨原
        /// </summary>
        public Nullable<int> Cause { get; set; }
        /// <summary>
        /// 退貨原因備註
        /// </summary>
        public string CauseNote { get; set; }
        /// <summary>
        /// 退貨處理方式
        /// </summary>
        public Nullable<int> DealWith { get; set; }
        /// <summary>
        /// 退貨處理日期
        /// </summary>
        public Nullable<System.DateTime> DealFinaldate { get; set; }
        /// <summary>
        ///  退出子單序號
        /// </summary>
        public Nullable<int> CheckinItemID { get; set; }
        /// <summary>
        /// 出庫子單序號
        /// </summary>
        public Nullable<int> StockOutItemID { get; set; }
        /// <summary>
        /// 退回對象
        /// </summary>
        public Nullable<int> Target { get; set; }
        /// <summary>
        /// 退貨處理備註
        /// </summary>
        public string DealNote { get; set; }
        /// <summary>
        /// 郵資
        /// </summary>
        public Nullable<int> PostAge { get; set; }
        /// <summary>
        /// 銀行名稱
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 銀行分行名稱
        /// </summary>
        public string BankBranch { get; set; }
        /// <summary>
        /// 銀行帳號
        /// </summary>
        public string AccountNO { get; set; }
        /// <summary>
        /// 銀行戶名
        /// </summary>
        public string AccountName { get; set; }
        public string CartID { get; set; }
        /// <summary>
        /// 訂購子單序號
        /// </summary>
        public string ProcessID { get; set; }
        /// <summary>
        /// 退款金額
        /// </summary>
        public Nullable<int> AmntRefund2c { get; set; }
        /// <summary>
        /// 退出宅配檔
        /// </summary>
        public string OutFileName { get; set; }
        /// <summary>
        /// 配件
        /// </summary>
        public string ATTS { get; set; }
        /// <summary>
        /// 取貨客戶姓名
        /// </summary>
        public string FrmName { get; set; }
        /// <summary>
        /// 取貨客戶所在縣(市)
        /// </summary>
        public string FrmLocation { get; set; }
        /// <summary>
        /// 取貨客戶郵遞區號
        /// </summary>
        public string FrmZipcode { get; set; }
        /// <summary>
        /// 取貨客戶地址
        /// </summary>
        public string FrmADDR { get; set; }
        /// <summary>
        /// 取貨客戶電話(日)
        /// </summary>
        public string FrmPhone { get; set; }
        /// <summary>
        /// 取貨客戶電話(夜)
        /// </summary>
        public string FrmPhone2 { get; set; }
        /// <summary>
        /// 取貨客戶電話(行)
        /// </summary>
        public string FrmMobile { get; set; }
        /// <summary>
        /// 取貨客戶 Email
        /// </summary>
        public string FrmEmail { get; set; }
        /// <summary>
        /// 取貨時段
        /// </summary>
        public Nullable<int> FrmTime { get; set; }
        /// <summary>
        /// 取貨備註
        /// </summary>
        public string FrmNote { get; set; }
        /// <summary>
        /// 配達公司名稱
        /// </summary>
        public string ToCompany { get; set; }
        /// <summary>
        /// 配達公司窗口
        /// </summary>
        public string ToName { get; set; }
        /// <summary>
        /// 配達公司所在縣(市)
        /// </summary>
        public Nullable<int> ToLocation { get; set; }
        /// <summary>
        /// 配達公司郵遞區號
        /// </summary>
        public string ToZipcode { get; set; }
        /// <summary>
        /// 配達公司地址
        /// </summary>
        public string ToADDR { get; set; }
        /// <summary>
        /// 配達公司電話
        /// </summary>
        public string ToPhone { get; set; }
        /// <summary>
        /// SCM
        /// </summary>
        public Nullable<int> ScmenaBled { get; set; }
        /// <summary>
        /// 供應商編號
        /// </summary>
        public string ScmNote { get; set; }
        /// <summary>
        /// 託運單號
        /// </summary>
        public string ShpCode { get; set; }
        /// <summary>
        /// 取貨匯出檔
        /// </summary>
        public string ShpEXPFile { get; set; }
        /// <summary>
        /// 入庫日期
        /// </summary>
        public Nullable<System.DateTime> StckDate { get; set; }
        /// <summary>
        /// 入庫人
        /// </summary>
        public string StckUser { get; set; }
        /// <summary>
        /// 作廢日期
        /// </summary>
        public Nullable<System.DateTime> EraseDate { get; set; }
        /// <summary>
        /// 作廢原因
        /// </summary>
        public Nullable<int> EraseCause { get; set; }
        /// <summary>
        /// 作廢原因備註
        /// </summary>
        public string EraseCauseNote { get; set; }
        /// <summary>
        /// 實際建檔日期
        /// </summary>
        public Nullable<System.DateTime> SysDate { get; set; }
        /// <summary>
        /// Updated
        /// </summary>
        public Nullable<int> Updated { get; set; }
        /// <summary>
        /// 更改日期
        /// </summary>
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        /// <summary>
        /// 最後更改人
        /// </summary>
        public string UpdatedUser { get; set; }
        public DateTime? OnReturnDate { get; set; }
        public DateTime? FinReturnDate { get; set; }
        public DateTime? ABNReturndate { get; set; }
        public DateTime? CancelReturnDate { get; set; }
        public string Note { get; set; }
        public string UpdateNote { get; set; }
        public Nullable<int> Declined { get; set; }
        public Nullable<decimal> FreightalLocation { get; set; }
        public Nullable<decimal> REShipping { get; set; }
        public Nullable<decimal> TaxCost { get; set; }
        public Nullable<int> ASNNumber { get; set; }
        public string SendStatus { get; set; }
        public Nullable<System.DateTime> SendDate { get; set; }
        public Nullable<int> InventoryStatus { get; set; }
        public Nullable<DateTime> InventoryStatusDate { get; set; }
        public string InventoryStatusUser { get; set; }
        /// <summary>
        /// 未開箱、已開箱、已損壞
        /// </summary>
        public Nullable<int> ProductStatus { get; set; }
        public Nullable<int> ToSAP { get; set; }
        public string ABNReturnReason { get; set; }
        public string SettlementID { get; set; }  //2013.11.1 add columns by Ice    //2013.11.4 int => string modify by Ice
        public string CreateUser { get; set; } //2014.03.10 penny
        public string ChangeSalesOrderCode { get; set; } //2014.07.04 steven
    }
}