using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.AccountRepoAdapters.Interface;
using TWNewEgg.AccountServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Account;

namespace TWNewEgg.AccountServices
{
    public class GetMemberService : IGetMemberService
    {
        private IMemberRepoAdapter _memberRepoAdapter;
        private IAccountRepoAdapter _accountRepoAdapter;

        public GetMemberService(IMemberRepoAdapter memberRepoAdapter, IAccountRepoAdapter accountRepoAdapter)
        {
            this._memberRepoAdapter = memberRepoAdapter;
            this._accountRepoAdapter = accountRepoAdapter;
        }

        public CartMemberInfoDM GetCartMemberInfo(int accID)
        {
            CartMemberInfoDM getCartMemberInfo = new CartMemberInfoDM();
            try
            {
                Member getMember = this._memberRepoAdapter.GetMember(accID);
                List<AddressBook> getAddressBookList = this._memberRepoAdapter.GetAddressBookList(accID).ToList();
                List<CompanyBook> getCompanyBookList = this._memberRepoAdapter.GetCompanyBookList(accID).ToList();
                if (getMember != null)
                {
                    getCartMemberInfo.MemberBase = ModelConverter.ConvertTo<MemberDM>(getMember);
                }

                if (getAddressBookList != null)
                {
                    //List<AddressBookDM> convertAddressBookBase = ModelConverter.ConvertTo<List<AddressBookDM>>(getAddressBookList);
                    //List<AddressBookDM> memberAddressBookList = convertAddressBookBase.Where(x => x.IsMemberBook == 1).OrderByDescending(x => x.CreateDate).ToList();
                    //List<AddressBookDM> deliverAddressBookList = convertAddressBookBase.Where(x => x.IsMemberBook != 1).OrderByDescending(x => x.CreateDate).ToList();

                    List<AddressBook> memberAddressBookList = getAddressBookList.Where(x => x.IsMemberBook == 1).OrderByDescending(x => x.DefaultSetting).ThenByDescending(x => x.UpdateDate).ToList();
                    List<AddressBook> deliverAddressBookList = getAddressBookList.Where(x => x.IsMemberBook != 1).OrderByDescending(x => x.DefaultSetting).ThenByDescending(x => x.UpdateDate).ToList();

                    if (memberAddressBookList.Count == 0 && getMember != null)
                    {
                        AddressBook addMemberAddressBook = new AddressBook();
                        Account getAccount = new Account();
                        getAccount = this._accountRepoAdapter.GetAccountByID(accID);
                        if (getAccount != null 
                            && getMember.Lastname != null 
                            && getMember.Firstname != null 
                            && getMember.Mobile != null
                            && getMember.Loc != null
                            && getMember.Zip != null
                            && getMember.Address != null)
                        {
                            addMemberAddressBook.AccountID = accID;
                            addMemberAddressBook.AccountEmail = getAccount.Email;
                            addMemberAddressBook.RecvSex = getMember.Sex ?? 2;
                            addMemberAddressBook.RecvName = getMember.Lastname + getMember.Firstname;
                            addMemberAddressBook.RecvFirstName = getMember.Firstname;
                            addMemberAddressBook.RecvLastName = getMember.Lastname;
                            addMemberAddressBook.RecvTelDay = "(" + getMember.TelZip + ")" + getMember.TelDay + "#" + getMember.TelExtension;
                            addMemberAddressBook.TelZip = getMember.TelZip;
                            addMemberAddressBook.TelDay = getMember.TelDay;
                            addMemberAddressBook.TelExtension = getMember.TelExtension;
                            addMemberAddressBook.RecvMobile = getMember.Mobile;
                            addMemberAddressBook.DelivLOC = getMember.Loc != null ? getMember.Loc.Trim() : "";
                            addMemberAddressBook.DelivZip = getMember.Zip != null ? getMember.Zip.Trim() : "";
                            addMemberAddressBook.DelivZipName = getMember.Zipname != null ? getMember.Zipname.Trim() : "";
                            addMemberAddressBook.DelivAddress = getMember.Address;
                            addMemberAddressBook.DelivAddr = addMemberAddressBook.DelivZipName + addMemberAddressBook.DelivAddress;
                            addMemberAddressBook.CreateDate = DateTime.UtcNow.AddHours(8);
                            addMemberAddressBook.DefaultSetting = 1;
                            addMemberAddressBook.IsMemberBook = 1;
                            this._memberRepoAdapter.AddAddressBook(addMemberAddressBook);
                            //AddressBookDM convertAddressBookDM = ModelConverter.ConvertTo<AddressBookDM>(addMemberAddressBook);
                            //memberAddressBookList.Add(convertAddressBookDM);
                            memberAddressBookList.Add(addMemberAddressBook);
                        }
                    }

                    //DataReforming(ref memberAddressBookList);
                    //DataReforming(ref deliverAddressBookList);
                    getCartMemberInfo.MemberAddressBookList = DataReforming(memberAddressBookList);
                    getCartMemberInfo.DeliverAddressBookList = DataReforming(deliverAddressBookList);
                }

                if (getCompanyBookList != null && getCompanyBookList.Count > 0)
                {
                    List<CompanyBook> companyBookDefaultList = getCompanyBookList.Where(x => x.DefaultSetting == 1).ToList();
                    if (companyBookDefaultList.Count == 0)
                    {
                        CompanyBook settingDefaultCompanyBook = getCompanyBookList.OrderByDescending(x => x.Updatedate).ToList().FirstOrDefault();
                        settingDefaultCompanyBook.DefaultSetting = 1;
                        this._memberRepoAdapter.ModifyCompanyBook(settingDefaultCompanyBook);
                        List<CompanyBook> getNewCompanyBookList = this._memberRepoAdapter.GetCompanyBookList(accID).ToList();
                        getCartMemberInfo.CompanyBookBaseList = ModelConverter.ConvertTo<List<CompanyBookDM>>(getNewCompanyBookList);
                    }
                    else if (companyBookDefaultList.Count > 1)
                    {
                        int count = 0;
                        companyBookDefaultList.OrderByDescending(x => x.Updatedate).ToList().ForEach(y =>
                        {
                            if (count == 0)
                            {
                                y.DefaultSetting = 1;
                                count++;
                            }
                            else
                            {
                                y.DefaultSetting = 0;
                            }
                            this._memberRepoAdapter.ModifyCompanyBook(y);
                        });
                        List<CompanyBook> getNewCompanyBookList = this._memberRepoAdapter.GetCompanyBookList(accID).ToList();
                        getCartMemberInfo.CompanyBookBaseList = ModelConverter.ConvertTo<List<CompanyBookDM>>(getNewCompanyBookList);
                    }
                    else
                    {
                        getCartMemberInfo.CompanyBookBaseList = ModelConverter.ConvertTo<List<CompanyBookDM>>(getCompanyBookList);
                    }
                    getCartMemberInfo.CompanyBookBaseList = getCartMemberInfo.CompanyBookBaseList.OrderByDescending(x => x.DefaultSetting).ThenByDescending(x => x.Updatedate).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return getCartMemberInfo;
        }

        /// <summary>
        /// 資料重整
        /// </summary>
        /// <param name="addressBookList">欲重整的紀錄本資料</param>
        private List<AddressBookDM> DataReforming(List<AddressBook> addressBookList)
        {
            List<AddressBookDM> result = new List<AddressBookDM>();
            try
            {
                // 設定紀錄本預設選項
                if (addressBookList.Count > 0)
                {
                    List<AddressBook> findAddressbooks = null;
                    // 檢查使用者預設選項是否不只一項
                    findAddressbooks = addressBookList.Where(x => x.DefaultSetting == 1).ToList();
                    if (findAddressbooks.Count > 1)
                    {
                        int listCount = 0;
                        addressBookList.ForEach(x =>
                        {
                            if (listCount == 0)
                            {
                                x.DefaultSetting = 1;
                                listCount++;
                            }
                            else
                            {
                                x.DefaultSetting = 0;
                            }
                            this._memberRepoAdapter.ModifyAddressBook(x);
                        });
                    }
                    else if (findAddressbooks.Count == 0)
                    {
                        // 若使用者沒有設定預設值，則以使用者所有紀錄本資訊中的第一個為預設值
                        addressBookList[0].DefaultSetting = 1;
                        this._memberRepoAdapter.ModifyAddressBook(addressBookList[0]);
                    }
                }

                addressBookList.ForEach(x =>
                {
                    AddressBookDM addressBookDM = new AddressBookDM();
                    addressBookDM = ModelConverter.ConvertTo<AddressBookDM>(x);
                    //string[] tel = x.RecvTelDay.Split(new char[] { '(', ')', '#' }).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    //addressBookDM.TelZip = tel.Length > 0 ? tel[0] : "";
                    //addressBookDM.TelDay = tel.Length > 1 ? tel[1] : "";
                    //addressBookDM.TelExtension = tel.Length > 2 ? tel[2] : "";
                    result.Add(addressBookDM);
                });
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AddressBookDM AddAddressBook(AddressBookDM addressBook)
        {
            try
            {
                List<AddressBook> getAddressBookList = this._memberRepoAdapter.GetAddressBookList(addressBook.AccountID).ToList();
                List<AddressBook> memberAddressBookList = new List<AddressBook>();
                if (getAddressBookList != null && getAddressBookList.Count > 0)
                {
                    getAddressBookList.Where(x => x.IsMemberBook == addressBook.IsMemberBook && x.DefaultSetting == 1).ToList().ForEach(y =>
                    {
                        y.DefaultSetting = 0;
                        this._memberRepoAdapter.ModifyAddressBook(y);
                    });
                }

                AddressBook updateAddressBook = ModelConverter.ConvertTo<AddressBook>(addressBook);
                AddressBook resultAddressBook = this._memberRepoAdapter.AddAddressBook(updateAddressBook);
                AddressBookDM result = ModelConverter.ConvertTo<AddressBookDM>(resultAddressBook);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ModifyAddressBook(AddressBookDM addressBook)
        {
            try
            {
                AddressBook updateAddressBook = ModelConverter.ConvertTo<AddressBook>(addressBook);
                return this._memberRepoAdapter.ModifyAddressBook(updateAddressBook);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteAddressBook(int addressBookID)
        {
            try
            {
                return this._memberRepoAdapter.DeleteAddressBook(addressBookID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ChangeAddressBookDefault(int accID, int addressBookID, bool memberChange)
        {
            try
            {
                List<AddressBook> getAddressBookList = this._memberRepoAdapter.GetAddressBookList(accID).ToList();
                if (getAddressBookList != null)
                {
                    List<AddressBook> isMemberBookNull = getAddressBookList.Where(x => x.IsMemberBook == null).ToList();
                    if (isMemberBookNull == null || isMemberBookNull.Count == 0)
                    {
                        DefaultChange(memberChange, addressBookID, getAddressBookList);
                    }
                    else
                    {
                        isMemberBookNull.ForEach(x =>
                        {
                            if (x.IsMemberBook == null)
                            {
                                x.IsMemberBook = 0;
                                this._memberRepoAdapter.ModifyAddressBook(x);
                            }
                        });

                        List<AddressBook> reGetAddressBookList = this._memberRepoAdapter.GetAddressBookList(accID).ToList();
                        DefaultChange(memberChange, addressBookID, reGetAddressBookList);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        private void DefaultChange(bool memberChange, int addressBookID, List<AddressBook> addressBookList)
        {
            int isMemberBook = 0;
            if (memberChange)
            {
                isMemberBook = 1;
            }
            List<AddressBook> memberAddressBookList = addressBookList.Where(x => x.IsMemberBook == isMemberBook).OrderByDescending(x => x.CreateDate).ToList();
            memberAddressBookList.ForEach(x =>
            {
                if (x.DefaultSetting == 1 && x.ID != addressBookID)
                {
                    x.DefaultSetting = 0;
                    this._memberRepoAdapter.ModifyAddressBook(x);
                }
                else if (x.DefaultSetting == 0 && x.ID == addressBookID)
                {
                    x.DefaultSetting = 1;
                    this._memberRepoAdapter.ModifyAddressBook(x);
                }
            });
        }

        public CompanyBookDM AddCompanyBook(CompanyBookDM addCompanyBook)
        {
            try
            {
                CompanyBook companyBook = ModelConverter.ConvertTo<CompanyBook>(addCompanyBook);
                CompanyBook addResult = this._memberRepoAdapter.AddCompanyBook(companyBook);
                CompanyBookDM convertResult = ModelConverter.ConvertTo<CompanyBookDM>(addResult);
                return convertResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ModifyCompanyBook(CompanyBookDM companyBookTemp)
        {
            try
            {
                CompanyBook updateCompanyBook = ModelConverter.ConvertTo<CompanyBook>(companyBookTemp);
                return this._memberRepoAdapter.ModifyCompanyBook(updateCompanyBook);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteCompanyBook(int companyBookID)
        {
            try
            {
                return this._memberRepoAdapter.DeleteCompanyBook(companyBookID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ChangeCompanyBookDefault(int accID, int companyBookID)
        {
            try
            {
                List<CompanyBook> getCompanyBookList = this._memberRepoAdapter.GetCompanyBookList(accID).ToList();
                if (getCompanyBookList != null)
                {
                    List<CompanyBook> companyBookDefaultList = getCompanyBookList.Where(x => x.DefaultSetting == 1).ToList();
                    companyBookDefaultList.ForEach(x => {
                        x.DefaultSetting = 0;
                        this._memberRepoAdapter.ModifyCompanyBook(x);
                    });
                    CompanyBook getCompanyBook = getCompanyBookList.Where(x => x.ID == companyBookID).FirstOrDefault();
                    if (getCompanyBook != null)
                    {
                        getCompanyBook.DefaultSetting = 1;
                        this._memberRepoAdapter.ModifyCompanyBook(getCompanyBook);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }
    }
}
