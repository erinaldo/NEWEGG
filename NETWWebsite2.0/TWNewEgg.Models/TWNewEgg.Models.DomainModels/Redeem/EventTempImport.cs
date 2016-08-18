using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Redeem
{
    public class EventTempImport
    {
        public EventTempImport()
        {
        }

        /// <summary>
        /// 編號, 非自動產生
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 活動id
        /// </summary>
        public int event_id { get; set; }

        /// <summary>
        /// 會員id
        /// </summary>
        public int account_id { get; set; }

        /// <summary>
        /// 會員email, 非必填
        /// </summary>
        public string account_email { get; set; }

        /// <summary>
        /// 使用狀態, default 0: 未使用, 1:已使用
        /// </summary>
        public int usageflag { get; set; }


    }//end class
}
