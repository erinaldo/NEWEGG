using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class NotificationOptionsData
    {
        public NotificationOptionsData()
        {
            this.orderNoticeOpen = false;
            this.orderNoticeClose = false;

            this.cancelNoticeOpen = false;
            this.cancelNoticeClose = false;

            this.returnsNoticeOpen = false;
            this.returnsNoticeClose = false;
        }

        public int SellerID { get; set; }
        public Nullable<int> InUserID { get; set; }

        public int orderNoticeSN { get; set; }
        public bool orderNoticeOpen { get; set; }
        public bool orderNoticeClose { get; set; }
        public string orderNoticeEmail_1st { get; set; }
        public string orderNoticeEmail_2nd { get; set; }
        public string orderNoticeEmail_3rd { get; set; }

        public int cancelNoticeSN { get; set; }
        public bool cancelNoticeOpen { get; set; }
        public bool cancelNoticeClose { get; set; }
        public string cancelNoticeEmail_1st { get; set; }
        public string cancelNoticeEmail_2nd { get; set; }
        public string cancelNoticeEmail_3rd { get; set; }

        public int businessNoticeSN { get; set; }
        public string businessNoticeEmail_1st { get; set; }
        public string businessNoticeEmail_2nd { get; set; }
        public string businessNoticeEmail_3rd { get; set; }

        public int financialNoticeSN { get; set; }
        public string financialNoticeEmail_1st { get; set; }
        public string financialNoticeEmail_2nd { get; set; }
        public string financialNoticeEmail_3rd { get; set; }

        public int returnsNoticeSN { get; set; }
        public bool returnsNoticeOpen { get; set; }
        public bool returnsNoticeClose { get; set; }
        public string returnsNoticeEmail_1st { get; set; }
        public string returnsNoticeEmail_2nd { get; set; }
        public string returnsNoticeEmail_3rd { get; set; }
    }
}