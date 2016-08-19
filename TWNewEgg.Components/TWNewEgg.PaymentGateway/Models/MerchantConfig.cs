using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.PaymentGateway.Models
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MerchantConfig
    {

        private string nameField;

        private string bankIDField;

        private int isOnceField;

        private string queryFlagField;

        private string hiServerURIField;

        private string merConfigURIField;

        private string updateURLField;

        private string merUpdateURLField;

        private string returnURLField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string BankID
        {
            get
            {
                return this.bankIDField;
            }
            set
            {
                this.bankIDField = value;
            }
        }

        /// <remarks/>
        public int IsOnce
        {
            get
            {
                return this.isOnceField;
            }
            set
            {
                this.isOnceField = value;
            }
        }

        /// <remarks/>
        public string QueryFlag
        {
            get
            {
                return this.queryFlagField;
            }
            set
            {
                this.queryFlagField = value;
            }
        }

        /// <remarks/>
        public string HiServerURI
        {
            get
            {
                return this.hiServerURIField;
            }
            set
            {
                this.hiServerURIField = value;
            }
        }

        /// <remarks/>
        public string MerConfigURI
        {
            get
            {
                return this.merConfigURIField;
            }
            set
            {
                this.merConfigURIField = value;
            }
        }

        /// <remarks/>
        public string UpdateURL
        {
            get
            {
                return this.updateURLField;
            }
            set
            {
                this.updateURLField = value;
            }
        }

        /// <remarks/>
        public string MerUpdateURL
        {
            get
            {
                return this.merUpdateURLField;
            }
            set
            {
                this.merUpdateURLField = value;
            }
        }

        /// <remarks/>
        public string ReturnURL
        {
            get
            {
                return this.returnURLField;
            }
            set
            {
                this.returnURLField = value;
            }
        }
    }
}
