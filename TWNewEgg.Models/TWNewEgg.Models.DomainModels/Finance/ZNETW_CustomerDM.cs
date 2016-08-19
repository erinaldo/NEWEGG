using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;

namespace TWNewEgg.Models.DomainModels.Finance
{
    //[Serializable]
    //[XmlRootAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Rfc/", IsNullable = false, ElementName = "ZNETW_CUSTOMER")]
    //[XmlTypeAttribute(AnonymousType = true, Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
    //public class CustomerDM
    //{
    //    public string KUNNR { get; set; }
    //    public string KTOKD { get; set; }
    //    public string LAND1 { get; set; }
    //    public string NAME1 { get; set; }
    //    public string NAME2 { get; set; }
    //    public string EMAIL { get; set; }
    //    public string ORT01 { get; set; }
    //    public string PSTLZ { get; set; }
    //    public string REGIO { get; set; }
    //    public string SORTL { get; set; }
    //    public string STRAS { get; set; }
    //    public string TELF1 { get; set; }
    //    public string TELF2 { get; set; }
    //    public string TELFX { get; set; }
    //    public string SPRAS { get; set; }
    //    public string STCD1 { get; set; }
    //    public string BUKRS { get; set; }
    //    public string AKONT { get; set; }
    //    public string ZTERM { get; set; }
    //    public string ZACTIONCODE { get; set; }
    //}



    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://Microsoft.LobServices.Sap/2007/03/Rfc/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Rfc/", IsNullable = false, ElementName="ZNETW_CUSTOMER")]
    public partial class ZNETW_CustomerDM
    {

        private ZNETW_CUSTOMERCUSTOMERDATA cUSTOMERDATAField;

        /// <remarks/>
        public ZNETW_CUSTOMERCUSTOMERDATA CUSTOMERDATA
        {
            get
            {
                return this.cUSTOMERDATAField;
            }
            set
            {
                this.cUSTOMERDATAField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://Microsoft.LobServices.Sap/2007/03/Rfc/")]
    public partial class ZNETW_CUSTOMERCUSTOMERDATA
    {

        private string kUNNRField;

        private string kTOKDField;

        private string lAND1Field;

        private string nAME1Field;

        private string nAME2Field;

        private string eMAILField;

        private string oRT01Field;

        private string pSTLZField;

        private string rEGIOField;

        private string sORTLField;

        private string sTRASField;

        private string tELF1Field;

        private string tELF2Field;

        private string tELFXField;

        private string sPRASField;

        private string sTCD1Field;

        private string bUKRSField;

        private string aKONTField;

        private string zTERMField;

        private string zACTIONCODEField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string KTOKD
        {
            get
            {
                return this.kTOKDField;
            }
            set
            {
                this.kTOKDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string LAND1
        {
            get
            {
                return this.lAND1Field;
            }
            set
            {
                this.lAND1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string NAME1
        {
            get
            {
                return this.nAME1Field;
            }
            set
            {
                this.nAME1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string NAME2
        {
            get
            {
                return this.nAME2Field;
            }
            set
            {
                this.nAME2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string ORT01
        {
            get
            {
                return this.oRT01Field;
            }
            set
            {
                this.oRT01Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string PSTLZ
        {
            get
            {
                return this.pSTLZField;
            }
            set
            {
                this.pSTLZField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string REGIO
        {
            get
            {
                return this.rEGIOField;
            }
            set
            {
                this.rEGIOField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
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
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string STRAS
        {
            get
            {
                return this.sTRASField;
            }
            set
            {
                this.sTRASField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string TELF1
        {
            get
            {
                return this.tELF1Field;
            }
            set
            {
                this.tELF1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string TELF2
        {
            get
            {
                return this.tELF2Field;
            }
            set
            {
                this.tELF2Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string TELFX
        {
            get
            {
                return this.tELFXField;
            }
            set
            {
                this.tELFXField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string SPRAS
        {
            get
            {
                return this.sPRASField;
            }
            set
            {
                this.sPRASField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string STCD1
        {
            get
            {
                return this.sTCD1Field;
            }
            set
            {
                this.sTCD1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string BUKRS
        {
            get
            {
                return this.bUKRSField;
            }
            set
            {
                this.bUKRSField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string AKONT
        {
            get
            {
                return this.aKONTField;
            }
            set
            {
                this.aKONTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string ZTERM
        {
            get
            {
                return this.zTERMField;
            }
            set
            {
                this.zTERMField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://Microsoft.LobServices.Sap/2007/03/Types/Rfc/")]
        public string ZACTIONCODE
        {
            get
            {
                return this.zACTIONCODEField;
            }
            set
            {
                this.zACTIONCODEField = value;
            }
        }
    }


}