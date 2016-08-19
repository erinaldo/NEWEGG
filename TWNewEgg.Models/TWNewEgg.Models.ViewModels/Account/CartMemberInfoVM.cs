using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Account
{
    public class CartMemberInfoVM
    {
        public MemberVM MemberBase { get; set; }
        public List<AddressBookVM> MemberAddressBookList { get; set; }
        public List<AddressBookVM> DeliverAddressBookList { get; set; }
        public List<CompanyBookVM> CompanyBookBaseList { get; set; }
    }
}
