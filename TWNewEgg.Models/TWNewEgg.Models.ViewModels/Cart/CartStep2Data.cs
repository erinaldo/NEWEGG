using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class CartStep2Data
    {
        public CartStep2Data()
        {
            this.memberUpdate = true;
            this.memberRecords = false;
            this.delivRecords = false;
            this.invoRecords = false;
        }

        public string SerialNumber { get; set; }
        public string Address { get; set; }
        public string Delivery { get; set; }
        public string OtherFoundation { get; set; }
        public string auth_code_3 { get; set; }
        public string cardCity { get; set; }
        public string cardaddr { get; set; }
        public string delivCity { get; set; }
        public string delivaddr { get; set; }
        public string expire_month { get; set; }
        public string expire_year { get; set; }
        public string invore3_2 { get; set; }
        public string invoreElec { get; set; }
        public string invoreElecCell { get; set; }
        public string invoreElecNatu { get; set; }
        public string member_firstname { get; set; }
        public string member_lastname { get; set; }
        public string member_recvfirstname { get; set; }
        public string member_recvlastname { get; set; }
        public string member_recvsex { get; set; }
        public string member_sex { get; set; }
        public string note { get; set; }
        public string recvteldayext { get; set; }
        public string recvteldaynumber { get; set; }
        public string recvteldayzip { get; set; }
        public string salesorder_cardaddr { get; set; }
        public string salesorder_cardloc { get; set; }
        public string salesorder_cardno { get; set; }
        public string salesorder_cardzip { get; set; }
        public string salesorder_delivaddr { get; set; }
        public string salesorder_delivloc { get; set; }
        public string salesorder_delivzip { get; set; }
        public string salesorder_invoid { get; set; }
        public string salesorder_invotitle { get; set; }
        public string salesorder_mobile { get; set; }
        public string salesorder_name { get; set; }
        public string salesorder_recvmobile { get; set; }
        public string salesorder_recvname { get; set; }
        public string salesorder_recvtelday { get; set; }
        public string salesorder_telday { get; set; }
        public string step1Data { get; set; }
        public string teldayext { get; set; }
        public string teldaynumber { get; set; }
        public string teldayzip { get; set; }
        public bool memberUpdate { get; set; }
        public bool memberRecords { get; set; }
        public bool delivRecords { get; set; }
        public bool invoRecords { get; set; }
        public string ArrivalTime { get; set; }
        // Electric Invoice 
        public string invoiceCarType { get; set; } //載具型態
        public string invoiceCarCell { get; set; } //手機載具碼
        public string invoiceCarNatu { get; set; } //自然人憑證碼
        public string invoiceDonCode { get; set; } //捐贈愛心碼
        /// <summary>
        /// 同意癈四機回收
        /// Y=同意, 預設NULL
        /// </summary>
        public string AgreedDiscard4 { get; set; }
    }
}
