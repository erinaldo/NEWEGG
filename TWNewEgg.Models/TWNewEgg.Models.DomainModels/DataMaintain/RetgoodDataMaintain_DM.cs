using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.DataMaintain
{
    public class RetgoodDataMaintain_DM
    {
        public int ID { get; set; }
        /// <summary>
        /// 退貨單號
        /// </summary>
        public string Code { get; set; }
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
        /// <summary>
        /// 退貨備註
        /// </summary>
        public string CauseNote { get; set; }
        /// <summary>
        /// 子單號碼
        /// </summary>
        public string ProcessID { get; set; }
        /// <summary>
        /// 退貨原因
        /// </summary>
        public Nullable<int> Cause { get; set; }
        /// <summary>
        /// 更新紀錄
        /// </summary>
        public string UpdateNote { get; set; }

    }
}
