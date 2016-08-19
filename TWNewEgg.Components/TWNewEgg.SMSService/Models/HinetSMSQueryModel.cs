﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SMSService.Models
{
    /// <summary>
    /// 中華電信SMS查詢資料模型
    /// </summary>
    public class HinetSMSQueryModel
    {
        private string _account = string.Empty;
        private string _password = string.Empty;
        private ToAddrTypeEnum _to_addr_type;
        private string _to_addr;
        private string _messageid;

        /// <summary>
        /// http網址
        /// </summary>
        public string httpurl
        {
            get
            {
                return "http://imsp.emome.net:8008/imsp/sms/servlet/QuerySM";
            }
        }

        /// <summary>
        /// https網址
        /// </summary>
        public string httpsurl
        {
            get
            {
                return "https://imsp.emome.net:4443/imsp/sms/servlet/QuerySM";
            }
        }

        /// <summary>
        /// 帳號
        /// </summary>
        public string account
        {
            get
            {
                return _account;
            }

            set
            {
                _account = value;
            }
        }

        /// <summary>
        /// 密碼
        /// </summary>
        public string password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        /// <summary>
        /// 發送對象號碼的種類
        /// </summary>
        public ToAddrTypeEnum to_addr_type
        {
            get
            {
                return _to_addr_type;
            }

            set
            {
                _to_addr_type = value;
            }
        }

        /// <summary>
        /// SMS訊息的發送對象的號碼
        /// 此欄可以包含一個以上的門號，以ASCII的逗號 (,) 隔開，最多接受20個獨立門號。
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
        /// SMS message的 message id，由IMSP SMS Server在成功的SMS傳送請求時傳回
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
    }
}