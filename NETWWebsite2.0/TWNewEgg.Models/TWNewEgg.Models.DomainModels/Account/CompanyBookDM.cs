using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Account
{
    public class CompanyBookDM
    {
        public CompanyBookDM()
        {
            this.DefaultSetting = 0;
            this.Title = "";
            this.Number = "";
            this.Delivloc = "";
            this.Delivzip = "";
            this.Delivaddr = "";
            this.DelivZipName = "";
            this.Createdate = DateTime.Now;
            this.Updated = 0;
        }

        public int ID { get; set; }
        public int Accountid { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string Delivloc { get; set; }
        public string Delivzip { get; set; }
        public string DelivZipName { get; set; }
        public string Delivaddr { get; set; }
        public Nullable<int> DefaultSetting { get; set; }
        public System.DateTime Createdate { get; set; }
        public int Updated { get; set; }
        public Nullable<System.DateTime> Updatedate { get; set; }
    }
}
