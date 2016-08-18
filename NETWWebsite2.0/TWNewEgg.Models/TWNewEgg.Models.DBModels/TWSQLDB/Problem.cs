using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("prblm")]
   public class Problem
    {
        public enum pbstatus
        {
            未處理 = 0,
            處理中 = 1,
            已結案 = 2,
            含未處理與處理中 = 3

        }
        public enum pbcase
        {
            訂購是否成功 = 101,
            更改訂單資料 = 108,
            查詢出貨進度 = 112,
            取消訂單 = 103,
            七天鑑賞期退貨 = 115,
            新品瑕疵換貨 = 116,
            商品缺件或不符 = 114,
            商品維修及保固 = 117,
            付款是否成功 = 102,
            退款問題 = 118,
            發票問題 = 119,
            折價券問題 = 120,
            商品規格 = 113,
            訂購及付款方式 = 121,
            行銷活動諮詢 = 105,
            系統網頁問題 = 106,
            詐騙相關問題 = 123,
            報關文件相關問題 = 124,
            其他問題 = 122
        };
        public enum pbsource
        {
            電話 = 0,
            信件 = 1,
            ERP = 2

        }
        public enum pborder
        {
            訂單 = 0,
            無訂單 = 1


        }
        public Problem()
        {

        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }
        public Nullable<int> AccountID { get; set; }
        public string PrblmCode { get; set; }
        public string Blng { get; set; }
        public string BlngCode { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> ProcessID { get; set; }
        public Nullable<int> SubStation { get; set; }
        public Nullable<int> Source { get; set; }
        public Nullable<int> Class { get; set; }
        public Nullable<int> IntClass { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string TEL { get; set; }
        public string Cont { get; set; }
        public string ProcessUser { get; set; }
        public string Subject { get; set; }
        public string ServerName { get; set; }
        public Nullable<System.DateTime> PreassignBeginDate { get; set; }
        public Nullable<System.DateTime> PreassignEndDate { get; set; }
        public Nullable<int> ScmdelvFailid { get; set; }
        public string Note { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusNote { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
