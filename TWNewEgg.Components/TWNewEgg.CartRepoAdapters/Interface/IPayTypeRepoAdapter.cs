using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface IPayTypeRepoAdapter
    {
        PayType GetPayType(int Id);
        IQueryable<PayType> GetAvailable();
        PayType GetPayTypeByPayType0rateNumandBankID(int? PayType, int? BankID);
        IQueryable<PayType> getAll();
        PayType GetPayTypeByBankCode(string bankCode);
    }
}
