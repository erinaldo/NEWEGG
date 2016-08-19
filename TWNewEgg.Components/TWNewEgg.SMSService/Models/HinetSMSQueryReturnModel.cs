using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SMSService.Models
{
    /// <summary>
    /// 中華電信SMS查詢回傳資料模型
    /// </summary>
    public class HinetSMSQueryReturnModel
    {
        private string _to_addr;
        private string _return_code;
        private string _done_time;
        private string _description;

        /// <summary>
        /// SMS訊息的發送對象的號碼
        /// </summary>
        public string to_addr
        {
            get
            {
                return _to_addr;
            }

            set
            {
                _to_addr = value;
            }
        }

        /// <summary>
        /// SMS訊息的回傳代碼
        /// </summary>
        public string return_code
        {
            get
            {
                return _return_code;
            }

            set
            {
                _return_code = value;
            }
        }

        /// <summary>
        /// SMS訊息的回傳完成時間
        /// </summary>
        public string done_time
        {
            get
            {
                return _done_time;
            }

            set
            {
                _done_time = value;
            }
        }

        /// <summary>
        /// SMS訊息的回傳訊息說明
        /// </summary>
        public string description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }
    }
}
