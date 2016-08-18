using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CommonService.DomainModels
{
    public class SMSSubmitReturnModel
    {
        private string _to_addr;
        private string _return_code;
        private string _messageid;
        private string _description;
        private int _smsID;

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
        /// SMS訊息的回傳訊息編號
        /// </summary>
        public string messageid
        {
            get
            {
                return _messageid;
            }

            set
            {
                _messageid = value;
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
        /// <summary>
        /// SMS ID
        /// </summary>
        public int smsID
        {
            get { return _smsID; }
            set { _smsID = value; }
        }
    }
}
