using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TWNewEgg.Models.DomainModels.Finance
{
    [Serializable]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://Microsoft.LobServices.Sap/2007/03/Rfc/")]
    [XmlRootAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Rfc/", IsNullable = false, ElementName="ZNETW_CUSTOMERResponse")]
    public partial class ZNETW_CUSTOMERResponseDM
    {

        private string kUNNRField;

        private string rETCODEField;

        private string rETMESGField;

        /// <remarks/>
        public string KUNNR
        {
            get
            {
                return this.kUNNRField;
            }
            set
            {
                this.kUNNRField = value;
            }
        }

        /// <remarks/>
        public string RETCODE
        {
            get
            {
                return this.rETCODEField;
            }
            set
            {
                this.rETCODEField = value;
            }
        }

        /// <remarks/>
        public string RETMESG
        {
            get
            {
                return this.rETMESGField;
            }
            set
            {
                this.rETMESGField = value;
            }
        }

        public ZNETW_CUSTOMERResponseDM() { }
    }
}
