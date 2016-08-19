using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.AccountRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.AccountRepoAdapters
{
    public class AccountRepoAdapter : IAccountRepoAdapter
    {
        private IRepository<Account> _accountDB;
        private IRepository<Member> _memberDB;

        public AccountRepoAdapter(IRepository<Account> account, IRepository<Member> member)
        {
            this._accountDB = account;
            this._memberDB = member;
        }

        public Account GetAccountByID(int accID)
        {
            Account account;

            if (accID == null)
            {
                return null;
            }

            account = _accountDB.Get(x => x.ID == accID);

            return account;
        }

        public Account GetAccount(string email)
        {
            Account account;

            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            account = _accountDB.Get(x => x.Email == email);

            return account;
        }

        public Account AddAccount(Account Accounttemp)
        {
            try
            {
                //檢查是否有輸入Email
                if (string.IsNullOrEmpty(Accounttemp.Email))
                {
                    throw new Exception("Email Null!!!");
                }
                //檢查是否有輸入GuestLogin
                if (Accounttemp.GuestLogin == null)
                {
                    throw new Exception("GuestLogin Null!!!");
                }
                //檢查是否有輸入GuestLogin
                if (Accounttemp.ReceiveEDM == null)
                {
                    throw new Exception("ReceiveEDM Null!!!");
                }

                //檢查Email是否已經存在
                Account account;
                account = _accountDB.Get(x => x.Email == Accounttemp.Email);
                if (account == null)
                {
                    Accounttemp.CreateDate = DateTime.Now;
                    Accounttemp.Registeron = Accounttemp.CreateDate;
                    Accounttemp.CreateUser = "System";
                    Accounttemp.Istosap = 0;
                    _accountDB.Create(Accounttemp);
                }
                else
                {
                    throw new Exception("Email Existed!!!");
                }

                return Accounttemp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Account GetGuestAccount(string email)
        {
            Account account;

            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            account = _accountDB.Get(x => x.Email == email && x.GuestLogin != 0);

            return account;
        }

        public Account GetNonGuestAccount(string email)
        {
            Account account;

            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            account = _accountDB.Get(x => x.Email == email && x.GuestLogin == 0);

            return account;
        }

        public Account GetAccountByEmailPass(string email, string password)
        {
            Account account;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            account = _accountDB.Get(x => x.Email == email && x.PWD == password);

            return account;
        }

        public Account UpdateAccount(Account newData)
        {
            _accountDB.Update(newData);
            return newData;
        }
    }
}
