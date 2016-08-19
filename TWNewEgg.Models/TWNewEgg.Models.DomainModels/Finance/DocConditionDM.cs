using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Finance
{
    public enum FinanDocTypeEnum
    {
        ALL,
        XQ,
        XD,
        XI,
        XIRMA
    }

    public class DocConditionDM
    {
        public FinanDocTypeEnum DocType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> SalesOrderCodeList { get; set; }

        public DocConditionDM()
        {
            SalesOrderCodeList = new List<string>();
        }        
    }  
}
