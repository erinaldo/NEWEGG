using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Bank
{
    public class Bank
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string Referred { get; set; }
    }

    public class PayType0rateNumAndBank
    {
        public PayType0rateNumAndBank()
        {
            //this.listBankDM = new List<Bank_DM>();
        }
        public int payTypeTableId { get; set; }
        public string Name { get; set; }
        public int PayType0rateNumber { get; set; }
        public string BankList { get; set; }
        public List<TWNewEgg.Models.ViewModels.Bank.Bank> listBankDM { get; set; }
    }
}
