using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Drawing;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using System.Data.SqlClient;

namespace TWNewEgg.Website.ECWeb.Service
{

    /// <summary>
    /// Hitrust直接授權的物件
    /// </summary>
    public class HitrustSSL
    {
        /// <summary>
        /// 建構函式
        /// </summary>
        public HitrustSSL()
        {
        }
        /// <summary>
        /// 訂單號碼
        /// </summary>
        public string ordernumber { get; set; }
        /// <summary>
        /// 交易金額: 須包含2位小數, EX: 輸入100, 代表1元
        /// </summary>
        public int amount { get; set; }
        
        /// <summary>
        /// 訂單說明
        /// </summary>
        public string orderdesc { get; set; }
        
        /// <summary>
        /// 機票號碼
        /// </summary>
        public string ticketno { get; set; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string pan { get; set; }
        /// <summary>
        /// 到期日(YYMM)
        /// </summary>
        public string expiry { get; set; }
        /// <summary>
        /// CVC2/ CVV2 (末三碼驗證碼)
        /// </summary>
        public string E01 { get; set; }
        /// <summary>
        /// 分期期數
        /// (無法與紅利折抵功能同時折抵)
        /// </summary>
        public string E03 { get; set; }
        /// <summary>
        /// 紅利折抵功能, 0:不啟用, 1:啟用
        /// (無法與分期功能同時使用)
        /// </summary>
        public string E04 { get; set; }
        /// <summary>
        /// 國民旅卡縣市群組代碼(見參考資料)
        /// 使用國民旅遊卡交易, 必須同時設定縣市群組代碼、啟程日、回程日
        /// </summary>
        public string E11 { get; set; }
        /// <summary>
        /// 國民旅遊卡: 啟程日(MMDDYYYY)'
        /// </summary>
        public string E12 { get; set; }
        /// <summary>
        /// 國民旅遊卡 回程日(MMDDYYYY)
        /// </summary>
        public string E13 { get; set; }
        /// <summary>
        /// 身份證字號
        /// </summary>
        public string E14 { get; set; }

        /// <summary>
        /// 回傳參數: 交易結果
        /// </summary>
        public string retcode { get; set; }
        
    }//end class

}//end namespace