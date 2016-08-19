using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CommonService.DomainModels;

namespace TWNewEgg.CommonService.Interface
{
    public interface INotificationService
    {
        /// <summary>
        /// 寄Email及簡訊
        /// </summary>
        void NotifyByMailAndSMS();

        /// <summary>
        /// 寄Email
        /// </summary>
        void NotifyByMail();

        /// <summary>
        /// 寄簡訊
        /// </summary>
        void NotifyBySMS();

        /// <summary>
        /// 設定訊息服務
        /// 若input的mail, phone無值，則用preset中的設定
        /// </summary>
        /// <param name="input"></param>
        void Set(NotificationModel input);
    }
}
