using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("HiTrustTrans")]
    public class HiTrustTrans
    {
        /// <summary>
        /// 商家一次付清的Conf路徑, EX: E:/HiTRUSTasp/HiTrustConf/61138.conf" 
        /// </summary>
        //[Key, Column("MerConfigName", Order = 0)]
        public string MerConfigName { get; set; }
        /// <summary>
        /// 訂單GtoupID號碼
        /// </summary>
        [Key, Column("ordernumber", Order = 0)]
        public string ordernumber { get; set; }
        /// <summary>
        /// Server Conf 路徑, EX: E:/HiTRUSTasp/HiTrustConf/HiServer.conf
        /// </summary>
        public string SerConfigName { get; set; }
        /// <summary>
        /// 交易金額: 須包含2位小數, EX: 輸入100, 代表1元
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 幣別
        /// </summary>
        public string currency { get; set; }
        /// <summary>
        /// 訂單說明
        /// </summary>
        public string orderdesc { get; set; }
        /// <summary>
        /// 自動請款，1：自動請款；0：一般交易
        /// </summary>
        public string depositflag { get; set; }
        /// <summary>
        /// 啟動查詢，1：詳細資料；0：一般資料
        /// </summary>
        public string queryflag { get; set; }
        /// <summary>
        /// 指定接續網址
        /// </summary>
        public string returnURL { get; set; }
        /// <summary>
        /// 交易結果網址
        /// </summary>
        public string merupdateURL { get; set; }
        /// <summary>
        /// 加密結果回傳網址
        /// </summary>
        public string updateURL { get; set; }
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
        /// 客戶端指定付款頁面功能代碼
        /// 1：英文付款頁面
        /// 2：中文付款頁面
        /// 3：中英文付款頁面
        /// </summary>
        public string E05 { get; set; }
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
        /// <summary>
        /// 回傳參數: 交易結果
        /// </summary>
        public string token { get; set; }

        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
