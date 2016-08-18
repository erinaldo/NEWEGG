using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
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
            [Description("商品故障/瑕疵")]商品故障瑕疵 = 6,
            [Description("商品寄錯/缺件")]商品寄錯缺件 = 7,
            [Description("商品尺寸或規格與網頁標示不同或不夠完整")]商品尺寸或規格與網頁標示不同或不夠完整 = 8,
            [Description("品質不佳(質感、性能等")]品質不佳質感性能等 = 9,
            [Description("商品不符所需或不適合")]商品不符所需或不適合 = 10,
            [Description("重複購買/不需要了")]重複購買不需要了 = 11,
            [Description("重訂(改買別款、改付款方式等)")]重訂改買別款改付款方式等 = 12,
            [Description("價格比別家貴")]價格比別家貴 = 13,
            [Description("同商品降價或有更優惠的活動")]同商品降價或有更優惠的活動 = 14,
            [Description("其他")]其他 = 15,
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

        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> CreatedFRM { get; set; }
        public Nullable<int> RetgoodType { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string RetgoodUser { get; set; }
        public Nullable<System.DateTime> FinalDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<System.DateTime> RECVGoodDate { get; set; }
        public Nullable<System.DateTime> RecvinvoDate { get; set; }
        public Nullable<int> InvocFmrslt { get; set; }
        public Nullable<System.DateTime> InvocFMDate { get; set; }
        public string InvocFMUser { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Cause { get; set; }
        public string CauseNote { get; set; }
        public Nullable<int> DealWith { get; set; }
        public Nullable<System.DateTime> DealFinaldate { get; set; }
        public Nullable<int> CheckinItemID { get; set; }
        public Nullable<int> StockOutItemID { get; set; }
        public Nullable<int> Target { get; set; }
        public string DealNote { get; set; }
        public Nullable<int> PostAge { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string AccountNO { get; set; }
        public string AccountName { get; set; }
        public string CartID { get; set; }
        public string ProcessID { get; set; }
        public Nullable<int> AmntRefund2c { get; set; }
        public string OutFileName { get; set; }
        public string ATTS { get; set; }
        public string FrmName { get; set; }
        public string FrmLocation { get; set; }
        public string FrmZipcode { get; set; }
        public string FrmADDR { get; set; }
        public string FrmPhone { get; set; }
        public string FrmPhone2 { get; set; }
        public string FrmMobile { get; set; }
        public string FrmEmail { get; set; }
        public Nullable<int> FrmTime { get; set; }
        public string FrmNote { get; set; }
        public string ToCompany { get; set; }
        public string ToName { get; set; }
        public Nullable<int> ToLocation { get; set; }
        public string ToZipcode { get; set; }
        public string ToADDR { get; set; }
        public string ToPhone { get; set; }
        public Nullable<int> ScmenaBled { get; set; }
        public string ScmNote { get; set; }
        public string ShpCode { get; set; }
        public string ShpEXPFile { get; set; }
        public Nullable<System.DateTime> StckDate { get; set; }
        public string StckUser { get; set; }
        public Nullable<System.DateTime> EraseDate { get; set; }
        public Nullable<int> EraseCause { get; set; }
        public string EraseCauseNote { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
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
        public Nullable<int> ProductStatus { get; set; }
        public Nullable<int> ToSAP { get; set; }
        public string ABNReturnReason { get; set; }
        public string SettlementID { get; set; }  //2013.11.1 add columns by Ice    //2013.11.4 int => string modify by Ice
        public string CreateUser { get; set; } //2014.03.10 penny
        public string ChangeSalesOrderCode { get; set; } //2014.07.04 steven
    }
}
