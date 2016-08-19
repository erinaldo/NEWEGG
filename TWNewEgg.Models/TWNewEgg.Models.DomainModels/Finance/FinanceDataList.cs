//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TWNewEgg.Models.DomainModels.Finance
//{
//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
//    public partial class FinanceDataList
//    {

//        private List<FinanceDataListFinanceData> financeDataField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("FinanceData")]
//        public List<FinanceDataListFinanceData> FinanceData
//        {
//            get
//            {
//                return this.financeDataField;
//            }
//            set
//            {
//                this.financeDataField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class FinanceDataListFinanceData
//    {

//        private System.DateTime startDateField;

//        private FinanceDataListFinanceDataVENDORDATA vENDORDATAField;

//        private FinanceDataListFinanceDataCUSTOMERDATA cUSTOMERDATAField;

//        private FinanceDataListFinanceDataDOCUMENTHEADER dOCUMENTHEADERField;

//        private List<string> textField;

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
//        public System.DateTime StartDate
//        {
//            get
//            {
//                return this.startDateField;
//            }
//            set
//            {
//                this.startDateField = value;
//            }
//        }

//        /// <remarks/>
//        public FinanceDataListFinanceDataVENDORDATA VENDORDATA
//        {
//            get
//            {
//                return this.vENDORDATAField;
//            }
//            set
//            {
//                this.vENDORDATAField = value;
//            }
//        }

//        /// <remarks/>
//        public FinanceDataListFinanceDataCUSTOMERDATA CUSTOMERDATA
//        {
//            get
//            {
//                return this.cUSTOMERDATAField;
//            }
//            set
//            {
//                this.cUSTOMERDATAField = value;
//            }
//        }

//        /// <remarks/>
//        public FinanceDataListFinanceDataDOCUMENTHEADER DOCUMENTHEADER
//        {
//            get
//            {
//                return this.dOCUMENTHEADERField;
//            }
//            set
//            {
//                this.dOCUMENTHEADERField = value;
//            }
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlTextAttribute()]
//        public List<string> Text
//        {
//            get
//            {
//                return this.textField;
//            }
//            set
//            {
//                this.textField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class FinanceDataListFinanceDataVENDORDATA
//    {

//        private string aCCT_GROUPField;

//        private string cOMPANYField;

//        private string pUR_ORGField;

//        private string cOUNTRYField;

//        private string lANGUAGEField;

//        private string cURRENCYField;

//        private string zTERMField;

//        private string rECON_ACCTField;

//        /// <remarks/>
//        public string ACCT_GROUP
//        {
//            get
//            {
//                return this.aCCT_GROUPField;
//            }
//            set
//            {
//                this.aCCT_GROUPField = value;
//            }
//        }

//        /// <remarks/>
//        public string COMPANY
//        {
//            get
//            {
//                return this.cOMPANYField;
//            }
//            set
//            {
//                this.cOMPANYField = value;
//            }
//        }

//        /// <remarks/>
//        public string PUR_ORG
//        {
//            get
//            {
//                return this.pUR_ORGField;
//            }
//            set
//            {
//                this.pUR_ORGField = value;
//            }
//        }

//        /// <remarks/>
//        public string COUNTRY
//        {
//            get
//            {
//                return this.cOUNTRYField;
//            }
//            set
//            {
//                this.cOUNTRYField = value;
//            }
//        }

//        /// <remarks/>
//        public string LANGUAGE
//        {
//            get
//            {
//                return this.lANGUAGEField;
//            }
//            set
//            {
//                this.lANGUAGEField = value;
//            }
//        }

//        /// <remarks/>
//        public string CURRENCY
//        {
//            get
//            {
//                return this.cURRENCYField;
//            }
//            set
//            {
//                this.cURRENCYField = value;
//            }
//        }

//        /// <remarks/>
//        public string ZTERM
//        {
//            get
//            {
//                return this.zTERMField;
//            }
//            set
//            {
//                this.zTERMField = value;
//            }
//        }

//        /// <remarks/>
//        public string RECON_ACCT
//        {
//            get
//            {
//                return this.rECON_ACCTField;
//            }
//            set
//            {
//                this.rECON_ACCTField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class FinanceDataListFinanceDataCUSTOMERDATA
//    {

//        private string kTOKDField;

//        private string lAND1Field;

//        private string sPRASField;

//        private string bUKRSField;

//        private string aKONTField;

//        private string zTERMField;

//        /// <remarks/>
//        public string KTOKD
//        {
//            get
//            {
//                return this.kTOKDField;
//            }
//            set
//            {
//                this.kTOKDField = value;
//            }
//        }

//        /// <remarks/>
//        public string LAND1
//        {
//            get
//            {
//                return this.lAND1Field;
//            }
//            set
//            {
//                this.lAND1Field = value;
//            }
//        }

//        /// <remarks/>
//        public string SPRAS
//        {
//            get
//            {
//                return this.sPRASField;
//            }
//            set
//            {
//                this.sPRASField = value;
//            }
//        }

//        /// <remarks/>
//        public string BUKRS
//        {
//            get
//            {
//                return this.bUKRSField;
//            }
//            set
//            {
//                this.bUKRSField = value;
//            }
//        }

//        /// <remarks/>
//        public string AKONT
//        {
//            get
//            {
//                return this.aKONTField;
//            }
//            set
//            {
//                this.aKONTField = value;
//            }
//        }

//        /// <remarks/>
//        public string ZTERM
//        {
//            get
//            {
//                return this.zTERMField;
//            }
//            set
//            {
//                this.zTERMField = value;
//            }
//        }
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class FinanceDataListFinanceDataDOCUMENTHEADER
//    {

//        private object oBJ_TYPEField;

//        private string bUS_ACTField;

//        private string uSERNAMEField;

//        private object oBJ_SYSField;

//        private object oBJ_KEYField;

//        private string cOMP_CODEField;

//        private string pROFIT_CTR_MarketingField;

//        private string pROFIT_CTR_LogField;

//        private string currentNumberField;

//        /// <remarks/>
//        public object OBJ_TYPE
//        {
//            get
//            {
//                return this.oBJ_TYPEField;
//            }
//            set
//            {
//                this.oBJ_TYPEField = value;
//            }
//        }

//        /// <remarks/>
//        public string BUS_ACT
//        {
//            get
//            {
//                return this.bUS_ACTField;
//            }
//            set
//            {
//                this.bUS_ACTField = value;
//            }
//        }

//        /// <remarks/>
//        public string USERNAME
//        {
//            get
//            {
//                return this.uSERNAMEField;
//            }
//            set
//            {
//                this.uSERNAMEField = value;
//            }
//        }

//        /// <remarks/>
//        public object OBJ_SYS
//        {
//            get
//            {
//                return this.oBJ_SYSField;
//            }
//            set
//            {
//                this.oBJ_SYSField = value;
//            }
//        }

//        /// <remarks/>
//        public object OBJ_KEY
//        {
//            get
//            {
//                return this.oBJ_KEYField;
//            }
//            set
//            {
//                this.oBJ_KEYField = value;
//            }
//        }

//        /// <remarks/>
//        public string COMP_CODE
//        {
//            get
//            {
//                return this.cOMP_CODEField;
//            }
//            set
//            {
//                this.cOMP_CODEField = value;
//            }
//        }

//        /// <remarks/>
//        public string PROFIT_CTR_Marketing
//        {
//            get
//            {
//                return this.pROFIT_CTR_MarketingField;
//            }
//            set
//            {
//                this.pROFIT_CTR_MarketingField = value;
//            }
//        }

//        /// <remarks/>
//        public string PROFIT_CTR_Log
//        {
//            get
//            {
//                return this.pROFIT_CTR_LogField;
//            }
//            set
//            {
//                this.pROFIT_CTR_LogField = value;
//            }
//        }

//        /// <remarks/>
//        public string CurrentNumber
//        {
//            get
//            {
//                return this.currentNumberField;
//            }
//            set
//            {
//                this.currentNumberField = value;
//            }
//        }
//    }
//}
