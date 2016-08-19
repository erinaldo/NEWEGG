using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Account;


namespace TWNewEgg.AccountServices.Interface
{
    public interface IAccountService
    {
        AccountInfoDM Login(string email, string password);
        AccountInfoDM UpdatePassword(string email, string enpryptPassword, string UpdateUser);
        AccountInfoDM GuestLogin(string email);
        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="AccountDM"></param>
        /// <returns></returns>
        AccountInfoDM Register(AccountDM AccountDM);
        string EmailExisted(string Email);
        bool EditAccount(string email, string email2);
        AccountDM UpdateAccount(AccountDM accountDM);
        AccountInfoDM EditPersonInfo(string email);
        bool EditPersonInformation(MemberDM memberDM, int edm, string email);
        MemberDM CreateMember(MemberDM memberDM);
        MemberDM GetMemberDMByEmail(string argStrEmail);
        MemberDM GetMember(int accountID);
        bool UpdateMemberInfo(MemberDM memberDM);
    }
}
