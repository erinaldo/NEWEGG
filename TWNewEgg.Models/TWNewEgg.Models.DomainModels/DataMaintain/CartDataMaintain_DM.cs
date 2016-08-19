using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.DataMaintain
{
    public class CartDataMaintain_DM
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string Receiver { get; set; }
        /// <summary>
        /// 收件人郵遞區號
        /// </summary>
        public string Zipcode { get; set; }
        /// <summary>
        /// 收件人所在縣市
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 受件人住址
        /// </summary>
        public string ADDR { get; set; }
        /// <summary>
        /// 收件人電話
        /// </summary>
        public string TelDay { get; set; }
        /// <summary>
        /// 收件人(夜)
        /// </summary>
        public string TelNight { get; set; }
        /// <summary>
        /// 收件人行動電話
        /// </summary>
        public string RecvMobile { get; set; }
        public string RecvENGName { get; set; }
        public string DelivENGADDR { get; set; }
        /// <summary>
        /// 購買人姓名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 訂購人電話(日)
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 訂購人電話(夜)
        /// </summary>
        public string Phone2 { get; set; }
        /// 訂購人行動電話
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 訂購人電子信箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 付款人電話(日)
        /// </summary>
        public string CardPhone { get; set; }
        /// <summary>
        /// 付款人電話(夜)
        /// </summary>
        public string CardPhone2 { get; set; }
        /// <summary>
        /// <summary>
        /// 信用卡帳單寄送地址郵遞區號
        /// </summary>
        public string CardZipcode { get; set; }
        /// <summary>
        /// 信用卡帳單寄送地址所在縣市
        /// </summary>
        public string CardLocation { get; set; }
        /// <summary>
        /// 信用卡帳單地址
        /// </summary>
        public string CardADDR { get; set; }
        /// <summary>
        /// /// <summary>
        /// 付款人行動電話
        /// </summary>
        public string CardMobile { get; set; }
        /// <summary>
        /// 發票抬頭
        /// </summary>
        public string IinvoiceTitle { get; set; }
        /// <summary>
        /// 發票統編
        /// </summary>
        public string ActCode { get; set; }
        /// <summary>
        /// 發票郵遞區號
        /// </summary>
        public string InvoZipcode { get; set; }
        /// <summary>
        ///  發票縣市
        /// </summary>
        public string InvoLocation { get; set; }
        /// <summary>
        /// 發票地址
        /// </summary>
        public string InvoADDR { get; set; }
        /// <summary>
        /// 發票收件人
        /// </summary>
        public string InvoReceiver { get; set; }
        //購買人縣市
        public Nullable<int> UsrLOC { get; set; }
        /// <summary>
        /// 購買人郵遞區號
        /// </summary>
        public string UserZipcode { get; set; }
        /// <summary>
        /// 購買人地址
        /// </summary>
        public string UsrADDR { get; set; }
        public Nullable<int> Updated { get; set; }
        /// <summary>
        /// 最後修改人
        /// </summary>
        public string UpdateUser { get; set; }
        /// <summary>
        /// 最後修改日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }
        /// <summary>
        /// 交易模式
        /// </summary>
        public Nullable<int> ShipType { get; set; }
        /// <summary>
        /// 更新紀錄
        /// </summary>
        public string UpdateNote { get; set; }
        /// <summary>
        /// 配達時間
        /// </summary>
        public string OrderNote { get; set; }
    }
}
