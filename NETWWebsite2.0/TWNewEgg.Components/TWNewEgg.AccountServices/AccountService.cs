using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.AccountServices.Interface;
using TWNewEgg.AccountRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.Account;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Framework.ServiceApi.Configuration;

using TWNewEgg.AccountEnprypt.Interface;


namespace TWNewEgg.AccountServices
{
    public class AccountService : IAccountService
    {
        private IAccountRepoAdapter _accountRepoAdapter;
        private IMemberRepoAdapter _memberRepoAdapter;
        private IAes _aes;

        public AccountService(IAccountRepoAdapter accountRepoAdapter, IMemberRepoAdapter memberRepoAdapter, IAes aes)
        {
            this._accountRepoAdapter = accountRepoAdapter;
            this._memberRepoAdapter = memberRepoAdapter;
            this._aes = aes;
        }

        public AccountInfoDM Login(string email, string enpryptPassword)
        {
            AccountInfoDM accountInfo = new AccountInfoDM();

            var validAccount = this._accountRepoAdapter.GetAccountByEmailPass(email, enpryptPassword);

            if (validAccount == null)
            {
                return null;
            }

            validAccount.Loginon = ConfigurationManager.GetTaiwanTime();
            this._accountRepoAdapter.UpdateAccount(validAccount);

            Member validMember = this._memberRepoAdapter.GetMember(validAccount.ID);

            accountInfo.ADM = ModelConverter.ConvertTo<AccountDM>(validAccount);
            if (validMember != null)
            {
                accountInfo.MDM = ModelConverter.ConvertTo<MemberDM>(validMember);
            }
            return accountInfo;
        }

        public AccountInfoDM GuestLogin(string email)
        {
            AccountInfoDM accountInfo = new AccountInfoDM();

            var validAccount = this._accountRepoAdapter.GetGuestAccount(email);

            if (validAccount == null)
            {
                return null;
            }

            validAccount.Loginon = ConfigurationManager.GetTaiwanTime();
            this._accountRepoAdapter.UpdateAccount(validAccount);

            Member validMember = this._memberRepoAdapter.GetMember(validAccount.ID);

            accountInfo.ADM = ModelConverter.ConvertTo<AccountDM>(validAccount);
            if (validMember != null)
            {
                accountInfo.MDM = ModelConverter.ConvertTo<MemberDM>(validMember);
            }
            return accountInfo;
        }

        public AccountInfoDM UpdatePassword(string email, string enpryptPassword, string UpdateUser)
        {
            AccountInfoDM accountInfo = new AccountInfoDM();

            var validAccount = this._accountRepoAdapter.GetAccount(email);

            validAccount.PWDtxt = validAccount.PWD;
            validAccount.PWD = enpryptPassword;
            validAccount.Updated = validAccount.Updated + 1;
            validAccount.UpdateUser = UpdateUser;
            validAccount.UpdateDate = DateTime.UtcNow.AddHours(8);
            this._accountRepoAdapter.UpdateAccount(validAccount);

            Member validMember = this._memberRepoAdapter.GetMember(validAccount.ID);

            accountInfo.ADM = ModelConverter.ConvertTo<AccountDM>(validAccount);
            if (validMember != null)
            {
                accountInfo.MDM = ModelConverter.ConvertTo<MemberDM>(validMember);
            }
            return accountInfo;
        }
        /// <summary>
        /// 網頁註冊
        /// </summary>
        /// <param name="AccountDM">AccountDM Model</param>
        /// <returns></returns>
        public AccountInfoDM Register(AccountDM AccountDM)
        {            
            try
            {
                //選告輸出
                AccountInfoDM accountInfo = new AccountInfoDM();
                TWNewEgg.Models.DBModels.TWSQLDB.Account Account = ModelConverter.ConvertTo<Account>(AccountDM);
                TWNewEgg.Models.DBModels.TWSQLDB.Account Accounttemp = this._accountRepoAdapter.AddAccount(Account);
                TWNewEgg.Models.DBModels.TWSQLDB.Member Member = ModelConverter.ConvertTo<Member>(Accounttemp);
                TWNewEgg.Models.DBModels.TWSQLDB.Member Membertemp = this._memberRepoAdapter.AddMember(Member);
                accountInfo.ADM = ModelConverter.ConvertTo<AccountDM>(Accounttemp);
                accountInfo.MDM = ModelConverter.ConvertTo<MemberDM>(Membertemp);
                return accountInfo;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Email是否存在
        /// </summary>
        /// <param name="AccountDM">AccountDM Model</param>
        /// <returns></returns>
        public string EmailExisted(string Email)
        {
            try
            {
                //選告輸出
                TWNewEgg.Models.DBModels.TWSQLDB.Account Account = this._accountRepoAdapter.GetAccount(Email);
                if (Account != null) {
                    return "true";
                }
                return "false";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AccountDM GetAccountByEmail(string Email)
        {
            try
            {
                //選告輸出
                TWNewEgg.Models.DBModels.TWSQLDB.Account Account = this._accountRepoAdapter.GetAccount(Email);

                if (Account != null)
                {
                    AccountDM accountDM = ModelConverter.ConvertTo<AccountDM>(Account);
                    return accountDM;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 帳號修改
        /// </summary>
        /// <param name="email">原始Email</param>
        /// <param name="email2">修改Email</param>
        /// <returns></returns>
        public bool EditAccount(string email, string email2)
        {
            var existedAccount = this.EmailExisted(email2);
            if (existedAccount == "true")
            {
                return false;
            }
            var validAccount = this._accountRepoAdapter.GetAccount(email);
            if (validAccount.Email == email2)
            {
                return false;
            }
            else
            {
                validAccount.Email = email2;
                validAccount.Email2 = email;
                validAccount.Updated = validAccount.Updated + 1;
                validAccount.UpdateUser = email;
                validAccount.UpdateDate = DateTime.UtcNow.AddHours(8);
                var result = this._accountRepoAdapter.UpdateAccount(validAccount);
                return true;
            }
        }

        public AccountDM UpdateAccount(AccountDM accountDM)
        {
            AccountDM result = new AccountDM();
            Account account = ModelConverter.ConvertTo<Account>(accountDM);
            TWNewEgg.Models.DBModels.TWSQLDB.Account Accounttemp = this._accountRepoAdapter.UpdateAccount(account);
            result = ModelConverter.ConvertTo<AccountDM>(Accounttemp);

            return result;
        }

        /// <summary>
        /// 修改個人資料
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns></returns>
        public AccountInfoDM EditPersonInfo(string email)
        {
            AccountInfoDM accountInfo = new AccountInfoDM();

            MemberDM memberDM = new MemberDM();
            var validAccount = this._accountRepoAdapter.GetAccount(email);
            if (validAccount == null)
            {
                return null;
            }
            accountInfo.ADM = ModelConverter.ConvertTo<AccountDM>(validAccount);
            //var member = this._memberRepoAdapter.GetMember(validAccount.ID);
            TWNewEgg.Models.DBModels.TWSQLDB.Member Membertemp = this._memberRepoAdapter.GetMember(validAccount.ID);
            accountInfo.MDM = null;
            if (Membertemp != null)
            {
                accountInfo.MDM = ModelConverter.ConvertTo<MemberDM>(Membertemp);
            }

            //accountInfo.MDM = memberDM;
            return accountInfo;
        }


        public bool EditPersonInformation(MemberDM memberDM,int edm, string email)
        {
            var validAccount = this._accountRepoAdapter.GetAccount(email);
            if (validAccount == null)
            {
                return false;
            }
            var validMember = this._memberRepoAdapter.GetMember(validAccount.ID);
            //memberDM.AccID = validAccount.ID;
            TWNewEgg.Models.DBModels.TWSQLDB.Member member = ModelConverter.ConvertTo<Member>(memberDM);

            if (validMember != null)
            {
                //updatemember
                validAccount.ReceiveEDM = edm;
                validAccount.UpdateDate = DateTime.UtcNow.AddHours(8);
                var resultAccount = this._accountRepoAdapter.UpdateAccount(validAccount);
                validMember.Address = member.Address;
                validMember.Address_en = member.Address_en;
                validMember.Birthday = member.Birthday;
                validMember.Firstname = member.Firstname;
                validMember.Firstname_en = member.Firstname_en;
                validMember.Lastname = member.Lastname;
                validMember.Lastname_en = member.Lastname_en;
                validMember.Loc = member.Loc;
                validMember.Mobile = member.Mobile;
                validMember.ModifyDate = validAccount.UpdateDate.Value;
                validMember.Nickname = member.Nickname;
                validMember.Sex = member.Sex;
                validMember.TelDay = member.TelDay;
                validMember.TelExtension = member.TelExtension;
                validMember.TelZip = member.TelZip;
                validMember.Zip = member.Zip;
                validMember.Zipname = member.Zipname;
                TWNewEgg.Models.DBModels.TWSQLDB.Member result = this._memberRepoAdapter.UpdateMember(validMember);
                return true;
            }
            else
            {
                //addmember
                validAccount.ReceiveEDM = edm;
                validAccount.UpdateDate = DateTime.UtcNow.AddHours(8);
                var resultAccount = this._accountRepoAdapter.UpdateAccount(validAccount);
                member.AccID = validAccount.ID;
                TWNewEgg.Models.DBModels.TWSQLDB.Member Membertemp = this._memberRepoAdapter.AddMember(member);
                return true;
            }
           
        }

        public MemberDM CreateMember(MemberDM memberDM)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.Member member = ModelConverter.ConvertTo<Member>(memberDM);
            TWNewEgg.Models.DBModels.TWSQLDB.Member newMember = this._memberRepoAdapter.AddMember(member);
            MemberDM result = null;
            if (newMember != null)
            {
                result = ModelConverter.ConvertTo<MemberDM>(newMember);
            }
            return result;
        }

        public MemberDM GetMemberDMByEmail(string argStrEmail)
        {
            AccountInfoDM objInfo = null;
            MemberDM objMemberDM = null;

            objInfo = this.EditPersonInfo(argStrEmail);
            if (objInfo != null && objInfo.IsSuccess)
            {
                objMemberDM = objInfo.MDM;
            }

            return objMemberDM;
        }
        public MemberDM GetMember(int accountID)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.Member member = this._memberRepoAdapter.GetMember(accountID);

            MemberDM result = null;
            if (member != null)
            {
                result = ModelConverter.ConvertTo<MemberDM>(member);
            }
            return result;
        }

        public bool UpdateMemberInfo(MemberDM memberDM)
        {
            var validMember = this._memberRepoAdapter.GetMember(memberDM.AccID);
            //memberDM.AccID = validAccount.ID;
            TWNewEgg.Models.DBModels.TWSQLDB.Member member = ModelConverter.ConvertTo<Member>(memberDM);

            if (validMember != null)
            {
                //updatemember
                validMember.Address = member.Address;
                validMember.Address_en = member.Address_en;
                validMember.Birthday = member.Birthday;
                validMember.Firstname = member.Firstname;
                validMember.Firstname_en = member.Firstname_en;
                validMember.Lastname = member.Lastname;
                validMember.Lastname_en = member.Lastname_en;
                validMember.Loc = member.Loc;
                validMember.Mobile = member.Mobile;
                validMember.ModifyDate = DateTime.UtcNow.AddHours(8);
                validMember.Nickname = member.Nickname;
                validMember.Sex = member.Sex;
                validMember.TelDay = member.TelDay;
                validMember.TelExtension = member.TelExtension;
                validMember.TelZip = member.TelZip;
                validMember.Zip = member.Zip;
                validMember.Zipname = member.Zipname;
                TWNewEgg.Models.DBModels.TWSQLDB.Member result = this._memberRepoAdapter.UpdateMember(validMember);
                return true;
            }

            return false;
        }

    }
}
