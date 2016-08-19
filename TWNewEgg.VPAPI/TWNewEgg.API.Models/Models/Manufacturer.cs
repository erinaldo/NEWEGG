using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    #region 大Model (DB的所有欄位)
    public class Manufacturer
    {
        // DB資料編號
        public int SN { get; set; }

        // 商家ID
        [DisplayName("商家")]
        public int SellerID { get; set; }

        // 審核結果通知對象ID 
        // 1.一般使用者建立時，通知對象為建立人
        // 2.管理者建立時，可於 Email To 欄位指定通知對象
        // 3.此欄位一但選定，無法藉由前台操作畫面更改
        public int? UserID { get; set; }

        // 製造商狀態
        // 1. P (Peding，審核中)
        // 2. A (Approved，核准)
        // 3. D (Decline，拒絕)
        // 4. L (Delete，作癈)
        [DisplayName("狀態")]
        public string ManufactureStatus { get; set; }

        // 製造商名稱
        [DisplayName("製造商名稱")]
        public string ManufactureName { get; set; }
        
        // 製造商網址
        [DisplayName("製造商網址")]
        public string ManufactureURL { get; set; }
        
        // 製造商支援信箱
        [DisplayName("製造商支援信箱")]
        public string SupportEmail { get; set; }
        
        // 製造商支援電話區碼
        public string PhoneRegion { get; set; }
        
        // 製造商支援電話
        [DisplayName("製造商支援電話")]
        public string Phone { get; set; }
        
        // 製造商支援電話分機碼
        public string PhoneExt { get; set; }
        
        // 製造商支援網址 (備用網址)
        [DisplayName("製造商支援網址")]
        public string supportURL { get; set; }
        
        // 拒絕製造商申請原因代號，0：存在相同製造商名稱，1：製造商已被拒絕過，2：其它
        public int? DeclineReasonType { get; set; }
        
        // 拒絕說明
        [DisplayName("拒絕原因")]
        public string DeclineReason { get; set; }
        
        // 資料更新日期
        [DisplayName("請求日期")]
        public DateTime? UpdateDate { get; set; }
        
        // 資料更新人ID
        public int? UpdateUserID { get; set; }
        
        // 資料建立日期
        public DateTime? InDate { get; set; }
        
        // 資料建立人ID
        public int? InUserID { get; set; }
    }
    #endregion 大Model (DB的所有欄位)

    #region 小Model (SellerPortal呈現資料用)
    /// <summary>
    /// Silverlight SellerPortal List 表格呈現資料用 Model
    /// </summary>
    public class ManufacturerListResultModel
    {
        // DB資料編號
        public int SN { get; set; }

        // 商家ID
        public int SellerID { get; set; }

        // 製造商狀態
        public string ManufactureStatus { get; set; }

        // 製造商名稱
        public string ManufactureName { get; set; }
        
        // 製造商網址
        public string ManufactureURL { get; set; }

        // 製造商支援信箱
        public string SupportEmail { get; set; }

        // 製造商支援電話區碼
        public string PhoneRegion { get; set; }

        // 製造商支援電話
        public string Phone { get; set; }

        // 製造商支援電話分機碼
        public string PhoneExt { get; set; }
        
        // 製造商支援網址
        public string supportURL { get; set; }

        // 更新日期 (顯示於 Request 頁面的請求日期)
        public DateTime? UpdateDate { get; set; }

        // 拒絕原因
        public string DeclineReason { get; set; }
    }
    #endregion 小Model (SellerPortal呈現資料用)

    #region Message
    /// <summary>
    /// 結果訊息暫存空間
    /// </summary>
    public class ManufacturerTempResultMessage
    {
        //成功訊息暫存空間
        public string SuccessMessage { get; set; }

        //失敗訊息暫存空間
        public string ErrorMessage { get; set; }
    }
    #endregion Message

    #region Search
    public class SearchDataModel
    {
        // 搜尋方式
        public SearchType SearchType { get; set; }

        // 搜尋關鍵字
        public string KeyWord { get; set; }

        // 搜尋指定 SellerID (若值為 0 時，示為不指定 SellerID，並顯示所有 SellerID 之結果)
        public int SellerID { get; set; }

        // 搜尋指定狀態
        public string Status { get; set; }

        // 是否將 SN 欄位列入搜尋
        public bool SearchSN { get; set; }
    }

    /// <summary>
    /// 搜尋方式
    /// </summary>
    public enum SearchType
    {
        // 僅列出狀態為 Approve 的結果
        Approve,

        // 依使用者指定的 狀態、關鍵字、SellerID 來顯示搜尋結果
        Selection,

        // 供批次建立查詢正式主檔的製造商資料 by URL
        SearchofficialInfobyURL,

        // 搜尋正式主檔內全部資料，供外面查詢
        SearchofficialALLInfo
    }
    #endregion Search

    #region MailTo
    /// <summary>
    /// GetEmailToList 回饋資料用 Model
    /// </summary>
    public class ManufacturerEmailToListResultModel
    {
        // 使用者ID
        public int UserID { get; set; }

        // 使用者電子信箱
        public string UserEmail { get; set; }
    }
    #endregion MailTo

    #region Validate
    /// <summary>
    /// 驗證資訊
    /// </summary>
    public class ManufacturerValidateInfo
    {
        // 驗證製造商名稱結果
        public bool ManufactureName { get; set; }

        // 驗證製造商支援信箱結果
        public bool SupportEmail { get; set; }

        // 驗證製造商支援網址結果
        public bool supportURL { get; set; }

        // 驗證總結
        public ManufacturerValidateSummaryResult SummaryResult
        { 
            get
            {
                // 如果全部驗證項目都為 ture，才算驗證成功
                if (ManufactureName
                 && SupportEmail
                 && supportURL)
                {
                    return ManufacturerValidateSummaryResult.Success;
                }
                else
                {
                    return ManufacturerValidateSummaryResult.Error;
                }
            }
        }
    }

    /// <summary>
    /// 驗證結果
    /// </summary>
    public enum ManufacturerValidateSummaryResult
    { 
        // 驗證通過
        Success,

        // 驗證失敗
        Error
    }
    #endregion Validate

    #region UpdateStatus Model
    /// <summary>
    /// 審核資訊
    /// </summary>
    public class ManufacturerUpdateStatusInfo
    {
        // 更新人ID
        public int? UpdateUserID { get; set; }

        // 更新日期
        public DateTime? UpdateDate { get; set; }

        // 審核項目
        public List<ManufacturerUpdateStatusData> UpdateList { get; set; }

        // 審核指令
        public ManufacturerUpdateStatusCommand? Command { get; set; }

        #region public string StatusValue
        private string statusValue;
        public string StatusValue
        {
            get
            {
                switch (Command)
                {
                    case ManufacturerUpdateStatusCommand.Approve:
                        {
                            statusValue = "A";
                            break;
                        }
                    case ManufacturerUpdateStatusCommand.Decline:
                        {
                            statusValue = "D";
                            break;
                        }
                    case ManufacturerUpdateStatusCommand.Delete:
                        {
                            statusValue = "L";
                            break;
                        }
                    default:
                        {
                            statusValue = null;
                            break;
                        }
                }
                return statusValue;
            }
        }
        #endregion
        #region public string ChineseResult
        private string chineseResult;
        public string ChineseResult
        {
            get
            {
                switch (Command)
                {
                    case ManufacturerUpdateStatusCommand.Approve:
                        {
                            chineseResult = "核准";
                            break;
                        }
                    case ManufacturerUpdateStatusCommand.Decline:
                        {
                            chineseResult = "拒絕";
                            break;
                        }
                    case ManufacturerUpdateStatusCommand.Delete:
                        {
                            chineseResult = "作廢";
                            break;
                        }
                    default:
                        {
                            chineseResult = null;
                            break;
                        }
                }
                return chineseResult;
            }
        }
        #endregion
        #region public string EnglishResult
        private string englishResult;
        public string EnglishResult
        {
            get
            {
                switch (Command)
                {
                    case ManufacturerUpdateStatusCommand.Approve:
                        {
                            englishResult = ManufacturerUpdateStatusCommand.Approve.ToString();
                            break;
                        }
                    case ManufacturerUpdateStatusCommand.Decline:
                        {
                            englishResult = ManufacturerUpdateStatusCommand.Decline.ToString();
                            break;
                        }
                    case ManufacturerUpdateStatusCommand.Delete:
                        {
                            englishResult = ManufacturerUpdateStatusCommand.Delete.ToString();
                            break;
                        }
                    default:
                        {
                            englishResult = null;
                            break;
                        }
                }
                return englishResult;
            }
        }
        #endregion
    }

    /// <summary>
    /// 待審核資料
    /// </summary>
    public class ManufacturerUpdateStatusData
    {
        // 製造商網址
        public string ManufactureURL { get; set; }
        
        // 製造商名稱
        public string ManufactureName { get; set; }

        // 拒絕原因
        public string DeclineReason { get; set; }
    }

    /// <summary>
    /// 審核結果通知信資訊
    /// </summary>
    public class ManufacturerUpdateStatusSendMailInfo
    {
        // 製造商網址
        public string ManufactureURL { get; set; }

        // 製造商名稱
        public string ManufactureName { get; set; }

        // 拒絕原因
        public string DeclineReason { get; set; }

        // 寄送類別
        public ManufacturerUpdateStatusSendType? SendType { get; set; }

        // 寄送類別中文內容
        #region public string DataType_Chinese
        private string sendType_Chinese;
        public string SendType_Chinese
        {
            get
            {
                switch (SendType)
                {
                    case ManufacturerUpdateStatusSendType.Create:
                        {
                            sendType_Chinese = "申請";
                            break;
                        }
                    case ManufacturerUpdateStatusSendType.Edit:
                        {
                            sendType_Chinese = "修改";
                            break;
                        }
                    case ManufacturerUpdateStatusSendType.Modified:
                    case ManufacturerUpdateStatusSendType.StatusSwitch:
                        {
                            sendType_Chinese = "建立";
                            break;
                        }
                    default:
                        {
                            sendType_Chinese = null;
                            break;
                        }
                }
                return sendType_Chinese;
            }
        }
        #endregion public string EditType_Chinese

        // 資料修改人
        public int? EditUser { get; set; }
        
        // 資料擁有人
        public int? DataOwner { get; set; }
        
        // 是否審核成功 (用來判斷是否寄送審核結果通知信)
        public bool IsSuccess { get; set; }

        // 審核失敗原因
        public ManufacturerUpdateStatusErrorType? ErrorMessage { get; set; }
    }

    /// <summary>
    /// 審核通知信寄送模式
    /// </summary>
    public enum ManufacturerUpdateStatusSendType
    {
        // 建立
        Create,

        // 編輯 (建立者編輯)
        Edit,

        // 被編輯 (非建立者編輯)
        Modified,

        // 切換製造商狀態 (無編輯，僅切換狀態)
        StatusSwitch
    }

    /// <summary>
    /// 審核指令
    /// </summary>
    public enum ManufacturerUpdateStatusCommand
    {
        // 核准
        Approve,

        // 拒絕
        Decline,

        // 作廢
        Delete
    }

    /// <summary>
    /// 審核失敗原因
    /// </summary>
    public enum ManufacturerUpdateStatusErrorType
    {
        // 資料不存在
        NotFindData,

        // 寫入錯誤
        SaveError
    }
    #endregion UpdateStatus Model
}