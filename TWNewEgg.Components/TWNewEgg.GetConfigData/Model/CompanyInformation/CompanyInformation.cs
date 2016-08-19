using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.GetConfigData.Models.CompanyInformation
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class VENDORList
    {

        private VENDORListVENDORDATA[] vENDORDATAField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("VENDORDATA")]
        public VENDORListVENDORDATA[] VENDORDATA
        {
            get
            {
                return this.vENDORDATAField;
            }
            set
            {
                this.vENDORDATAField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class VENDORListVENDORDATA
    {

        private string tYPEField;

        private string vENDORField;

        private string aCCT_GROUPField;

        private string cOMPANYField;

        private string pUR_ORGField;

        private string cOUNTRYField;

        private string sTATUSField;

        private string nAMEENGField;

        private string nAMEField;

        private string pOContactWithField;

        private string eMAILField;

        private string cITYField;

        private string cITYENGField;

        private string pOSTCODEField;

        private string sORTLField;

        private string aDDRESSField;

        private string aDDRESSENG1Field;

        private string aDDRESSENG2Field;

        private string tELField;

        private string tELFPO1Field;

        private string tELFPO2Field;

        private string tELFAXPOField;

        private string tELFAXField;

        private string vAT_NOField;

        /// <remarks/>
        public string TYPE
        {
            get
            {
                return this.tYPEField;
            }
            set
            {
                this.tYPEField = value;
            }
        }

        /// <remarks/>
        public string VENDOR
        {
            get
            {
                return this.vENDORField;
            }
            set
            {
                this.vENDORField = value;
            }
        }

        /// <remarks/>
        public string ACCT_GROUP
        {
            get
            {
                return this.aCCT_GROUPField;
            }
            set
            {
                this.aCCT_GROUPField = value;
            }
        }

        /// <remarks/>
        public string COMPANY
        {
            get
            {
                return this.cOMPANYField;
            }
            set
            {
                this.cOMPANYField = value;
            }
        }

        /// <remarks/>
        public string PUR_ORG
        {
            get
            {
                return this.pUR_ORGField;
            }
            set
            {
                this.pUR_ORGField = value;
            }
        }

        /// <remarks/>
        public string COUNTRY
        {
            get
            {
                return this.cOUNTRYField;
            }
            set
            {
                this.cOUNTRYField = value;
            }
        }

        /// <remarks/>
        public string STATUS
        {
            get
            {
                return this.sTATUSField;
            }
            set
            {
                this.sTATUSField = value;
            }
        }

        /// <remarks/>
        public string NAMEENG
        {
            get
            {
                return this.nAMEENGField;
            }
            set
            {
                this.nAMEENGField = value;
            }
        }

        /// <remarks/>
        public string NAME
        {
            get
            {
                return this.nAMEField;
            }
            set
            {
                this.nAMEField = value;
            }
        }

        /// <remarks/>
        public string POContactWith
        {
            get
            {
                return this.pOContactWithField;
            }
            set
            {
                this.pOContactWithField = value;
            }
        }

        /// <remarks/>
        public string EMAIL
        {
            get
            {
                return this.eMAILField;
            }
            set
            {
                this.eMAILField = value;
            }
        }

        /// <remarks/>
        public string CITY
        {
            get
            {
                return this.cITYField;
            }
            set
            {
                this.cITYField = value;
            }
        }

        /// <remarks/>
        public string CITYENG
        {
            get
            {
                return this.cITYENGField;
            }
            set
            {
                this.cITYENGField = value;
            }
        }

        /// <remarks/>
        public string POSTCODE
        {
            get
            {
                return this.pOSTCODEField;
            }
            set
            {
                this.pOSTCODEField = value;
            }
        }

        /// <remarks/>
        public string SORTL
        {
            get
            {
                return this.sORTLField;
            }
            set
            {
                this.sORTLField = value;
            }
        }

        /// <remarks/>
        public string ADDRESS
        {
            get
            {
                return this.aDDRESSField;
            }
            set
            {
                this.aDDRESSField = value;
            }
        }

        /// <remarks/>
        public string ADDRESSENG1
        {
            get
            {
                return this.aDDRESSENG1Field;
            }
            set
            {
                this.aDDRESSENG1Field = value;
            }
        }

        /// <remarks/>
        public string ADDRESSENG2
        {
            get
            {
                return this.aDDRESSENG2Field;
            }
            set
            {
                this.aDDRESSENG2Field = value;
            }
        }

        /// <remarks/>
        public string TEL
        {
            get
            {
                return this.tELField;
            }
            set
            {
                this.tELField = value;
            }
        }

        /// <remarks/>
        public string TELFPO1
        {
            get
            {
                return this.tELFPO1Field;
            }
            set
            {
                this.tELFPO1Field = value;
            }
        }

        /// <remarks/>
        public string TELFPO2
        {
            get
            {
                return this.tELFPO2Field;
            }
            set
            {
                this.tELFPO2Field = value;
            }
        }

        /// <remarks/>
        public string TELFAXPO
        {
            get
            {
                return this.tELFAXPOField;
            }
            set
            {
                this.tELFAXPOField = value;
            }
        }

        /// <remarks/>
        public string TELFAX
        {
            get
            {
                return this.tELFAXField;
            }
            set
            {
                this.tELFAXField = value;
            }
        }

        /// <remarks/>
        public string VAT_NO
        {
            get
            {
                return this.vAT_NOField;
            }
            set
            {
                this.vAT_NOField = value;
            }
        }
    }


}

