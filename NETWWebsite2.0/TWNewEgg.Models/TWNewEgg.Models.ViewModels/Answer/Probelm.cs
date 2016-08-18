using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Answer
{
    public enum status
   {
        訂購是否成功=101,
        更改訂單資料=108,
        查詢出貨進度=112,
        取消訂單=103,
        報關文件相關問題=124,
        七天鑑賞期退貨=115,
        新品瑕疵換貨 = 116,
        商品缺件或不符=114,
        商品維修及保固=117,
        付款是否成功=102,
        退款問題=118,
        發票問題=119,
        折價券問題=120,
        商品規格=113,
        訂購及付款方式=121,
        行銷活動諮詢=105,
        詐騙相關問題=123,
        其他問題=122,
        系統網頁問題=106


   }
    public class Probelm
    {
        public string Code { get; set; }
        public DateTime CreateDate { get; set; }
        public string BlngCode { get; set; }
        public string Name { get; set; }
        public string Cont { get; set; }
        public string Prblmtype { get; set; }
        public List<Answer> Answerlist { get; set; }
    }
    public class Answer
    {
        public string PrblmCode { get; set; }

        public string Cont { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public enum problemType
    {
        Salceorder = 1,
        Retgood = 2,
        Invoice = 3,
        Item = 4,
        Other = 5
    }
}
