using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("creditauth")]
    public class CreditAuth
    {
        //public CreditAuth()
        //{
        //    DateTime defaultDate = DateTime.Parse("1900/01/01 00:00:00");
        //    //Date = defaultDate;
        //    //CreateDate = DateTime.Now;
        //    //UpdateDate = defaultDate;
        //}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string Blng { get; set; }
        public Nullable<int> BlngID { get; set; }
        public Nullable<int> ProcessID { get; set; }
        public Nullable<int> AuthID { get; set; }
        public Nullable<int> PayGateway { get; set; }
        public string OrderitemCode { get; set; }
        public Nullable<int> DealType { get; set; }
        public string AcqBank { get; set; }
        public string CustomerID { get; set; }
        public string AgreementID { get; set; }
        public string UserID { get; set; }
        public string OrderNO { get; set; }
        public int SalesOrderGroupID { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Amnt { get; set; }
        public Nullable<decimal> AmountSelf { get; set; }
        public Nullable<decimal> AmountApply { get; set; }
        public Nullable<decimal> Bonus { get; set; }
        public Nullable<int> Bonusbln { get; set; }
        public Nullable<decimal> Bonusrate { get; set; }
        public string HpMark { get; set; }
        public Nullable<int> PriceFirst { get; set; }
        public Nullable<int> PriceOther { get; set; }
        public Nullable<System.DateTime> AuthDate { get; set; }
        public string AuthExpfile { get; set; }
        public string AuthCode { get; set; }
        public string AuthSN { get; set; }
        public Nullable<System.DateTime> AuthrspDate { get; set; }
        public string AuthrspimpFile { get; set; }
        public string AuthrspCode { get; set; }
        public string AuthrspMSG { get; set; }
        public string AuthrspOther { get; set; }
        public Nullable<System.DateTime> CancelDate { get; set; }
        public string CancelrspCode { get; set; }
        public string CancelrspMSG { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> EraseDate { get; set; }
        public string FaildealUser { get; set; }
        public string FaildealNote { get; set; }
        public Nullable<System.DateTime> FailDealDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string SuccessFlag { get; set; }
    }
}
