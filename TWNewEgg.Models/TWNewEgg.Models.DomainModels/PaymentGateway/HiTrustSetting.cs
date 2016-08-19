using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PaymentGateway
{
    public class HiTrustSetting
    {
        public enum isOnce
        {
            分期 = 1,
            一次 = 0
        }
        public enum queryFlag
        {
            詳細資料 = 1,
            一般資料 = 0
        }

        /// <summary>
        /// 商家ID EX:61909
        /// </summary>
        public string StoreID { get; set; }

        /// <summary>
        /// 商家一次付清的Conf路徑, EX: E:/HiTRUSTasp/HiTrustConf/61138.conf" 
        /// </summary>
        public string MerConfigName { get; set; }
        /// <summary>
        /// Server Conf 路徑, EX: E:/HiTRUSTasp/HiTrustConf/HiServer.conf
        /// </summary>
        public string SerConfigName { get; set; }
        /// <summary>
        /// 啟動查詢, 1: 詳細資料, 0:一般資料
        /// (啟動詳細資料時, Trust Pay Server會將交易詳細資料以POST的方式送至MerUpdateURL)
        /// </summary>
        public string queryflag { get; set; }
        /// <summary>
        /// 指定接續網址
        /// </summary>
        public string returnURL { get; set; }
        /// <summary>
        /// 交易結果網址 (啟動queryflag=1時, 交易資料會以POST方式傳至merupdateURL
        /// </summary>
        public string merupdateURL { get; set; }
        /// <summary>
        /// 加密結果回傳網址
        /// </summary>
        public string updateURL { get; set; }
    }
}
