using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class ManufactureUpdateStatusDataAdd : TWNewEgg.API.Models.ManufacturerUpdateStatusData
    {
        public ManufactureUpdateStatusDataAdd()
        {
            this.rejectAllDefault1 = false;
            this.rejectAllDefault2 = false;
            this.rejectAllDefault3 = false;
            this.rejectDefault1 = false;
            this.rejectDefault2 = false;
            this.rejectDefault3 = false;
        }

        public Nullable<int> SN { get; set; }
        public string UpdateCommand { get; set; }
        public bool rejectAllDefault1 { get; set; }
        public bool rejectAllDefault2 { get; set; }
        public bool rejectAllDefault3 { get; set; }
        public bool rejectDefault1 { get; set; }
        public bool rejectDefault2 { get; set; }
        public bool rejectDefault3 { get; set; }
    }
}
