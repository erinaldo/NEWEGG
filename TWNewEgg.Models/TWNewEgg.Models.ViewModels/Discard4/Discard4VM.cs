using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Discard4
{
    /// <summary>
    /// 癈四機同意
    /// </summary>
    public class Discard4VM
    {

        public Discard4VM() { }
        public Discard4VM(int salesOrderGroupID, string agreedDiscard4, string accountEmail )
        {
            this.SalesOrderGroupID = salesOrderGroupID;
            this.AgreedDiscard4 = agreedDiscard4;
            this.AccountEmail = accountEmail;
        }

        /// <summary>
        /// 購物車編號
        /// </summary>
        public int SalesOrderGroupID { get; set; }

        /// <summary>
        /// 執行同意癈四機回收的按鈕
        /// Y=同意, 預設NULL
        /// </summary>
        public string AgreedDiscard4 { get; set; }

        /// <summary>
        /// 購車的使用者MAIL
        /// </summary>
        public string AccountEmail { get; set; }


    }
}
