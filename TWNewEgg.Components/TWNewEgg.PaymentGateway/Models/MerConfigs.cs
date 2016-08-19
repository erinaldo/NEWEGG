using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.PaymentGateway.Models
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MerConfigs
    {

        private MerchantConfig[] merchantField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Merchant")]
        public MerchantConfig[] Merchants
        {
            get
            {
                return this.merchantField;
            }
            set
            {
                this.merchantField = value;
            }
        }
    }
}
