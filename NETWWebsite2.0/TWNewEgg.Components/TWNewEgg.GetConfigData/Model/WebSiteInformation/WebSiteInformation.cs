using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.GetConfigData.Service;

namespace TWNewEgg.GetConfigData.Models
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class WebSiteList
    {

        private List<WebSiteListWebSiteData> webSiteDataField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("WebSiteData")]
        public List<WebSiteListWebSiteData> WebSiteData
        {
            get
            {
                return this.webSiteDataField;
            }
            set
            {
                this.webSiteDataField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class WebSiteListWebSiteData
    {
        public WebSiteListWebSiteData()
        {
        }

        public WebSiteListWebSiteData(int ID)
        {
            WebSiteInformation WebSiteInformation = new WebSiteInformation();
            TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData();
            WebSiteData = WebSiteInformation.GetWebSiteData(ID, "WebSiteInformation.config");
            this.Abbreviation =  WebSiteData.Abbreviation;
            this.Address1 = WebSiteData.Address1;
            this.Address1_Eng = WebSiteData.Address1_Eng;
            this.Address2 = WebSiteData.Address2;
            this.Address2_Eng = WebSiteData.Address2_Eng;
            this.City = WebSiteData.City;
            this.City_Eng = WebSiteData.City_Eng;
            this.CompanyName = WebSiteData.CompanyName;
            this.CompanyName_Eng = WebSiteData.CompanyName_Eng;
            this.Contact_Eng = WebSiteData.Contact_Eng;
            this.Contact1 = WebSiteData.Contact1;
            this.Contact2 = WebSiteData.Contact2;
            this.Country = WebSiteData.Country;
            this.Country_Eng = WebSiteData.Country_Eng;
            this.Email1 = WebSiteData.Email1;
            this.Email2 = WebSiteData.Email2;
            this.EmailSenderName = WebSiteData.EmailSenderName;
            this.Fax1 = WebSiteData.Fax1;
            this.Fax1_Eng = WebSiteData.Fax1_Eng;
            this.Fax2 = WebSiteData.Fax2;
            this.Fax2_Eng = WebSiteData.Fax2_Eng;
            this.ID = WebSiteData.ID;
            this.PhoneNumber1 = WebSiteData.PhoneNumber1;
            this.PhoneNumber1_Eng = WebSiteData.PhoneNumber1_Eng;
            this.PhoneNumber2_Eng = WebSiteData.PhoneNumber2_Eng;
            this.PhoneNumber2 = WebSiteData.PhoneNumber2;
            this.Shortname_Eng = WebSiteData.Shortname_Eng;
            this.SiteName = WebSiteData.SiteName;
            this.SiteName_Eng = WebSiteData.SiteName_Eng;
            this.VATNumber = WebSiteData.VATNumber;
            this.Zipcode = WebSiteData.Zipcode;
            this.ServiceTime = WebSiteData.ServiceTime;
            this.PhoneNumber3 = WebSiteData.PhoneNumber3;

        }

        private int idField;

        private string companyNameField;

        private string siteNameField;

        private string abbreviationField;

        private string vATNumberField;

        private string contact1Field;

        private string contact2Field;

        private string phoneNumber1Field;

        private string phoneNumber2Field;

        private string fax1Field;

        private string fax2Field;

        private string address1Field;

        private string address2Field;

        private string cityField;

        private string countryField;

        private string zipcodeField;

        private string email1Field;

        private string email2Field;

        private string emailSenderNameField;

        private string companyName_EngField;

        private string siteName_EngField;

        private string shortname_EngField;

        private string contact_EngField;

        private string phoneNumber1_EngField;

        private string phoneNumber2_EngField;

        private string fax1_EngField;

        private string fax2_EngField;

        private string address1_EngField;

        private string address2_EngField;

        private string city_EngField;

        private string country_EngField;

        private string serviceTimeField;

        private string phoneNumber3Field;
        /// <remarks/>
        public int ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string CompanyName
        {
            get
            {
                return this.companyNameField;
            }
            set
            {
                this.companyNameField = value;
            }
        }

        /// <remarks/>
        public string SiteName
        {
            get
            {
                return this.siteNameField;
            }
            set
            {
                this.siteNameField = value;
            }
        }

        /// <remarks/>
        public string Abbreviation
        {
            get
            {
                return this.abbreviationField;
            }
            set
            {
                this.abbreviationField = value;
            }
        }

        /// <remarks/>
        public string VATNumber
        {
            get
            {
                return this.vATNumberField;
            }
            set
            {
                this.vATNumberField = value;
            }
        }

        /// <remarks/>
        public string Contact1
        {
            get
            {
                return this.contact1Field;
            }
            set
            {
                this.contact1Field = value;
            }
        }

        /// <remarks/>
        public string Contact2
        {
            get
            {
                return this.contact2Field;
            }
            set
            {
                this.contact2Field = value;
            }
        }

        /// <remarks/>
        public string PhoneNumber1
        {
            get
            {
                return this.phoneNumber1Field;
            }
            set
            {
                this.phoneNumber1Field = value;
            }
        }

        /// <remarks/>
        public string PhoneNumber2
        {
            get
            {
                return this.phoneNumber2Field;
            }
            set
            {
                this.phoneNumber2Field = value;
            }
        }

        /// <remarks/>
        public string Fax1
        {
            get
            {
                return this.fax1Field;
            }
            set
            {
                this.fax1Field = value;
            }
        }

        /// <remarks/>
        public string Fax2
        {
            get
            {
                return this.fax2Field;
            }
            set
            {
                this.fax2Field = value;
            }
        }

        /// <remarks/>
        public string Address1
        {
            get
            {
                return this.address1Field;
            }
            set
            {
                this.address1Field = value;
            }
        }

        /// <remarks/>
        public string Address2
        {
            get
            {
                return this.address2Field;
            }
            set
            {
                this.address2Field = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string Zipcode
        {
            get
            {
                return this.zipcodeField;
            }
            set
            {
                this.zipcodeField = value;
            }
        }

        /// <remarks/>
        public string Email1
        {
            get
            {
                return this.email1Field;
            }
            set
            {
                this.email1Field = value;
            }
        }

        /// <remarks/>
        public string Email2
        {
            get
            {
                return this.email2Field;
            }
            set
            {
                this.email2Field = value;
            }
        }

        /// <remarks/>
        public string EmailSenderName
        {
            get
            {
                return this.emailSenderNameField;
            }
            set
            {
                this.emailSenderNameField = value;
            }
        }

        /// <remarks/>
        public string CompanyName_Eng
        {
            get
            {
                return this.companyName_EngField;
            }
            set
            {
                this.companyName_EngField = value;
            }
        }

        /// <remarks/>
        public string SiteName_Eng
        {
            get
            {
                return this.siteName_EngField;
            }
            set
            {
                this.siteName_EngField = value;
            }
        }

        /// <remarks/>
        public string Shortname_Eng
        {
            get
            {
                return this.shortname_EngField;
            }
            set
            {
                this.shortname_EngField = value;
            }
        }

        /// <remarks/>
        public string Contact_Eng
        {
            get
            {
                return this.contact_EngField;
            }
            set
            {
                this.contact_EngField = value;
            }
        }

        /// <remarks/>
        public string PhoneNumber1_Eng
        {
            get
            {
                return this.phoneNumber1_EngField;
            }
            set
            {
                this.phoneNumber1_EngField = value;
            }
        }

        /// <remarks/>
        public string PhoneNumber2_Eng
        {
            get
            {
                return this.phoneNumber2_EngField;
            }
            set
            {
                this.phoneNumber2_EngField = value;
            }
        }

        /// <remarks/>
        public string Fax1_Eng
        {
            get
            {
                return this.fax1_EngField;
            }
            set
            {
                this.fax1_EngField = value;
            }
        }

        /// <remarks/>
        public string Fax2_Eng
        {
            get
            {
                return this.fax2_EngField;
            }
            set
            {
                this.fax2_EngField = value;
            }
        }

        /// <remarks/>
        public string Address1_Eng
        {
            get
            {
                return this.address1_EngField;
            }
            set
            {
                this.address1_EngField = value;
            }
        }

        /// <remarks/>
        public string Address2_Eng
        {
            get
            {
                return this.address2_EngField;
            }
            set
            {
                this.address2_EngField = value;
            }
        }

        /// <remarks/>
        public string City_Eng
        {
            get
            {
                return this.city_EngField;
            }
            set
            {
                this.city_EngField = value;
            }
        }

        /// <remarks/>
        public string Country_Eng
        {
            get
            {
                return this.country_EngField;
            }
            set
            {
                this.country_EngField = value;
            }
        }
        /// <remarks/>
        public string ServiceTime
        {
            get
            {
                return this.serviceTimeField;
            }
            set
            {
                this.serviceTimeField = value;
            }
        }

        /// <remarks/>
        public string PhoneNumber3
        {
            get
            {
                return this.phoneNumber3Field;
            }
            set
            {
                this.phoneNumber3Field = value;
            }
        }
    }


}

