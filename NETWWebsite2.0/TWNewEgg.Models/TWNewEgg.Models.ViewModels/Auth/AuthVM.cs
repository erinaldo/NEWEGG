using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Auth
{
    public class AuthVM
    {
        public AuthVM()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            AuthDate = defaultDate;
            CancelDate = defaultDate;
            FaildealDate = defaultDate;
            EraseDate = defaultDate;
            CreateDate = defaultDate;
            UpdateDate = defaultDate;
        }

        public int ID { get; set; }
        public string SalesOrderItemCode { get; set; }
        public string AcqBank { get; set; }
        public string CustomerID { get; set; }
        public string AgreementID { get; set; }
        public int AccountID { get; set; }
        public string OrderNO { get; set; }
        public int SalesOrderGroupID { get; set; }
        public int Qty { get; set; }
        public int Amount { get; set; }
        public int AmountSelf { get; set; }
        public int Bonus { get; set; }
        public int BonusBLN { get; set; }
        public string HpMark { get; set; }
        public int PriceFirst { get; set; }
        public int PriceOther { get; set; }
        public string AuthCode { get; set; }
        public string AuthSN { get; set; }
        public System.DateTime AuthDate { get; set; }
        public string RspCode { get; set; }
        public string RspMSG { get; set; }
        public string RspOther { get; set; }
        public System.DateTime CancelDate { get; set; }
        public string CancelRspCode { get; set; }
        public string CancelRspMSG { get; set; }
        public string FaildealUser { get; set; }
        public string FaildealNote { get; set; }
        public System.DateTime FaildealDate { get; set; }
        public System.DateTime EraseDate { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string SuccessFlag { get; set; }
    }
}
