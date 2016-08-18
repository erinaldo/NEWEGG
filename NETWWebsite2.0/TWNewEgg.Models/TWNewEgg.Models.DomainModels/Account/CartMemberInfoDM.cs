using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Account
{
    public class CartMemberInfoDM
    {
        public MemberDM MemberBase { get; set; }
        public List<AddressBookDM> MemberAddressBookList { get; set; }
        public List<AddressBookDM> DeliverAddressBookList { get; set; }
        public List<CompanyBookDM> CompanyBookBaseList { get; set; }
    }
}
