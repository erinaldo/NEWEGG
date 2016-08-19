using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.AccountRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.AccountRepoAdapters
{
    public class MemberRepoAdapter : IMemberRepoAdapter
    {
        private IRepository<Member> _memberDB;
        private IRepository<AddressBook> _addressBookDB;
        private IRepository<CompanyBook> _companyBookDB;

        public MemberRepoAdapter(IRepository<Member> member, IRepository<AddressBook> addressBook, IRepository<CompanyBook> companyBook)
        {
            this._memberDB = member;
            this._addressBookDB = addressBook;
            this._companyBookDB = companyBook;
        }

        public Member GetMember(int accountID)
        {
            Member member;
            if (accountID == 0)
            {
                return null;
            }

            member = _memberDB.Get(x => x.AccID == accountID);

            return member;
        }

        public Member AddMember(Member Membertemp)
        {
            try
            {
                //檢查是否有輸入AccID
                if (Membertemp.AccID == null)
                {
                    throw new Exception("AccID Null!!!");
                }
                //檢查Email是否已經存在
                Member member;
                member = _memberDB.Get(x => x.AccID == Membertemp.AccID);
                if (member == null)
                {
                    Membertemp.CreateDate = DateTime.Now;
                    Membertemp.ModifyDate = DateTime.Now;
                    _memberDB.Create(Membertemp);
                }
                else
                {
                    throw new Exception("AccID Existed!!!");
                }

                return Membertemp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Member UpdateMember(Member newMember)
        {
            _memberDB.Update(newMember);
            return newMember;
        }

        public IQueryable<AddressBook> GetAddressBookList(int accID)
        {
            return _addressBookDB.GetAll().Where(x => x.AccountID == accID);
        }

        public AddressBook AddAddressBook(AddressBook addressBookTemp)
        {
            try
            {
                // 檢查是否有輸入AccountID
                if (addressBookTemp.AccountID == null)
                {
                    throw new Exception("AccountID Null!!!");
                }

                addressBookTemp.CreateDate = DateTime.UtcNow.AddHours(8);
                _addressBookDB.Create(addressBookTemp);

                return addressBookTemp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ModifyAddressBook(AddressBook addressBookTemp)
        {
            if (addressBookTemp == null)
            {
                return false;
            }

            try
            {
                addressBookTemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                _addressBookDB.Update(addressBookTemp);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteAddressBook(int addressBookID)
        {
            if (addressBookID == null)
            {
                return false;
            }

            //檢查AddressBook是否存在
            AddressBook addressBook;
            try
            {
                addressBook = _addressBookDB.Get(x => x.ID == addressBookID);
                if (addressBook != null)
                {
                    _addressBookDB.Delete(addressBook);
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IQueryable<CompanyBook> GetCompanyBookList(int accID)
        {
            return _companyBookDB.GetAll().Where(x => x.Accountid == accID);
        }

        public CompanyBook AddCompanyBook(CompanyBook companyBookTemp)
        {
            try
            {
                // 檢查是否有輸入AccountID
                if (companyBookTemp.Accountid == null)
                {
                    throw new Exception("AccountID Null!!!");
                }

                companyBookTemp.Createdate = DateTime.UtcNow.AddHours(8);
                _companyBookDB.Create(companyBookTemp);

                return companyBookTemp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ModifyCompanyBook(CompanyBook companyBookTemp)
        {
            if (companyBookTemp == null)
            {
                return false;
            }

            try
            {
                companyBookTemp.Updatedate = DateTime.UtcNow.AddHours(8);
                _companyBookDB.Update(companyBookTemp);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteCompanyBook(int companyBookID)
        {
            if (companyBookID == null)
            {
                return false;
            }

            //檢查CompanyBook是否存在
            CompanyBook companyBook;
            try
            {
                companyBook = _companyBookDB.Get(x => x.ID == companyBookID);
                if (companyBook != null)
                {
                    _companyBookDB.Delete(companyBook);
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
