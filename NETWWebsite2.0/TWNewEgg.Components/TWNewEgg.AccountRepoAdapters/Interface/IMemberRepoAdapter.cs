using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.AccountRepoAdapters.Interface
{
    public interface IMemberRepoAdapter
    {
        Member GetMember(int accountID);
        Member AddMember(Member Membertemp);
        Member UpdateMember(Member newMember);
        IQueryable<AddressBook> GetAddressBookList(int accID);
        AddressBook AddAddressBook(AddressBook addressBookTemp);
        bool ModifyAddressBook(AddressBook addressBookTemp);
        bool DeleteAddressBook(int addressBookID);
        IQueryable<CompanyBook> GetCompanyBookList(int accID);
        CompanyBook AddCompanyBook(CompanyBook companyBookTemp);
        bool ModifyCompanyBook(CompanyBook companyBookTemp);
        bool DeleteCompanyBook(int companyBookID);
    }
}
