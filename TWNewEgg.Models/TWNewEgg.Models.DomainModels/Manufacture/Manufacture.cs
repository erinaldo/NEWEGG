using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Manufacture
{
    public class Manufacture
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? Showorder { get; set; }
        public int? Status { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public string Phone { get; set; }
        public string WebAddress { get; set; }
        public string Address { get; set; }
        public string SourceContry { get; set; }
        public Nullable<System.DateTime> Updatedate { get; set; }
        public string BrandStory { get; set; }
    }
}
