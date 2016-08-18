using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Models
{
    /// <summary>
    /// 各信通用Model
    /// </summary>
    public class Mail
    {
        public Mail()
        {
            this.OrderInfo = new Mail_OrderInfo();
        }

        /// <summary>
        /// 使用者信箱  (not null)
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 密件副本
        /// </summary>
        public string RecipientBcc { get; set; }

        /// <summary>
        /// 信件類型  (not null)
        /// </summary>
        public MailTypeEnum MailType { get; set; }

        /// <summary>
        /// 信件類型列舉
        /// </summary>
        public enum MailTypeEnum
        {
            SallerInvitationEmail,
            ManufactureRequestNotification, // 通知seller申請製造商的審核結果
            InventoryAlertEmail,
            InformSellerNewSalesOrder, //通知Seller有新的訂單 add by Ted
            InformSellerCancelOrder, // 訂單取消通知
            ResetPassword,
            NewManufactureVerifyEmail, //通知管理者有新的製造商申請
            Mail_TO_PMs,
            PriceChangedMail, // 通知PM Seller修改商品售價
            ErrorInfo,
            RemindSellerToSendPackage, //出貨提醒通知信,   
            RemindGrossMargin, // 毛利率低於館價通知信,
            UserResponse,
            RMAInfo, // RMA Vendor 通知信
            RMASuccessInfo // 退貨處理完成通知管理者
        }

        //2014.1.27 增加製造商信件資訊 by Smoke
        /// <summary>
        /// 製造商信件資訊
        /// </summary>
        public string MailMessage { get; set; }

        // 2014.05.06 增加信件商家名稱 by Jack
        /// <summary>
        /// 商家名稱
        /// </summary>
        public string UserName { get; set; }

        // 2014.07.03 add by Jack.C
        /// <summary>
        /// 寄送給 PM 的賣場資料
        /// </summary>
        public List<ItemInfoList> ItemInfoList { get; set; }

        /// <summary>
        /// 寄送給系統管理員的 提醒出貨通知信(訂單清單)
        /// </summary>
        public List<UnShipList> UnshipList { get; set; }

        // 2014.10.06 add by Ted
        /// <summary>
        /// 是否為管理員身分
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Mail 處理訂單相關所使用的欄位
        /// </summary>
        public Mail_OrderInfo OrderInfo { get; set; }
    }

    public class Mail_OrderInfo
    {
        public string OrderID { get; set; }

        /// <summary>
        /// 請撈取 Process 的 title，此為訂單成立所記錄的商品名稱
        /// </summary>
        public string ProductName { get; set; }
    }
}
