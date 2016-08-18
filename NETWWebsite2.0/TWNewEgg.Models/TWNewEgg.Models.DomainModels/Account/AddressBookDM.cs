using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Account
{
    public class AddressBookDM
    {
        public AddressBookDM()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            this.IsMemberBook = 0;
            this.CreateDate = defaultDate;
            this.UpdateDate = defaultDate;
        }

        public int ID { get; set; }
        public int AccountID { get; set; }
        public Nullable<int> RecvSex { get; set; }
        public string RecvName { get; set; }
        public string RecvFirstName { get; set; }
        public string RecvLastName { get; set; }
        public string RecvTelDay { get; set; }
        public string RecvMobile { get; set; }
        public string DelivLOC { get; set; }
        public string DelivZip { get; set; }
        public string DelivZipName { get; set; }
        public string DelivAddress { get; set; }
        public string DelivAddr { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string AccountEmail { get; set; }
        public int DefaultSetting { get; set; }
        public Nullable<int> IsMemberBook { get; set; }
        public string TelZip { get; set; }
        public string TelDay { get; set; }
        public string TelExtension { get; set; }
    }
}
