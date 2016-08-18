using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.AccountRepoAdapters.Interface
{
    public interface IAccountRepoAdapter
    {
        Account GetAccountByID(int accID);
        Account GetAccount(string email);
        Account AddAccount(Account Accounttemp);
        Account GetNonGuestAccount(string email);
        Account GetGuestAccount(string email);
        Account GetAccountByEmailPass(string email, string enpryptPassword);
        Account UpdateAccount(Account newData);        
    }
}
