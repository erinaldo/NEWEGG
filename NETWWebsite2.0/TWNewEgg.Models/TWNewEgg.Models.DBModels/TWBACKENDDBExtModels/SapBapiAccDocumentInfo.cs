using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.Models.DBModels.TWBACKENDDBExtModels
{
    public class SapBapiAccDocumentInfo
    {
        public AccountsDocumentType.DocTypeEnum DocType { get; set; }
        public Sap_BapiAccDocument_DocHeader DocHeader { get; set; }
        public IEnumerable<Sap_BapiAccDocument_DocDetail> DocDetail { get; set; }
    }
}
