using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PaymentGateway
{
    public class NCCCResult
    {
        public string OrderID { get; set; }
        public string PAN { get; set; }
        public string ExpireDate { get; set; }
        public string TransCode { get; set; }
        public string TransMode { get; set; }
        public string TransAmt { get; set; }
        public string TransDate { get; set; }
        public string TransTime { get; set; }
        public string ApproveCode { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public string Installment { get; set; }
        public string InstallmentType { get; set; }
        public string FirstAmt { get; set; }
        public string EachAmt { get; set; }
        public string Fee { get; set; }
        public string RedeemBalance { get; set; }
        public string RedeemType { get; set; }
        public string RedeemUsed { get; set; }
        public string CreditAmt { get; set; }
        public string RiskMark { get; set; }
        public string IsForeign { get; set; }
        public string SecureStatus { get; set; }
    }
}
