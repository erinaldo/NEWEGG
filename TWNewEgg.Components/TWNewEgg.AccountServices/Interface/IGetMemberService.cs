using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Account;

namespace TWNewEgg.AccountServices.Interface
{
    public interface IGetMemberService
    {
        CartMemberInfoDM GetCartMemberInfo(int accID);
        AddressBookDM AddAddressBook(AddressBookDM addressBook);
        bool ModifyAddressBook(AddressBookDM addressBook);
        bool DeleteAddressBook(int addressBookID);
        bool ChangeAddressBookDefault(int accID, int addressBookID, bool memberChange);
        CompanyBookDM AddCompanyBook(CompanyBookDM addCompanyBook);
        bool ModifyCompanyBook(CompanyBookDM companyBookTemp);
        bool DeleteCompanyBook(int companyBookID);
        bool ChangeCompanyBookDefault(int accID, int companyBookID);
    }
}
