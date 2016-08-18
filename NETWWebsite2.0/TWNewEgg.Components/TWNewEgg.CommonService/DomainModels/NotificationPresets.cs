using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CommonService.DomainModels
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class NotificationPresets
    {
        private NotificationPreset[] presetField;

        private string environment;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Preset")]
        public NotificationPreset[] Presets
        {
            get
            {
                return this.presetField;
            }
            set
            {
                this.presetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string env
        {
            get
            {
                return this.environment;
            }
            set
            {
                this.environment = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NotificationPreset
    {

        private NotificationMailPreset mailPresetField;

        private NotificationPhonePreset phonePresetField;

        private string idField;

        private string nameField;

        /// <remarks/>
        public NotificationMailPreset MailPreset
        {
            get
            {
                return this.mailPresetField;
            }
            set
            {
                this.mailPresetField = value;
            }
        }

        /// <remarks/>
        public NotificationPhonePreset PhonePreset
        {
            get
            {
                return this.phonePresetField;
            }
            set
            {
                this.phonePresetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NotificationMailPreset
    {

        private NotificationMail[] mailListField;

        private NotificationMail[] ccMailListField;

        private NotificationMail[] bccMailListField;

        private string contentField;

        private string titleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Mail", IsNullable = false)]
        public NotificationMail[] MailList
        {
            get
            {
                return this.mailListField;
            }
            set
            {
                this.mailListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Mail", IsNullable = false)]
        public NotificationMail[] ccMailList
        {
            get
            {
                return this.ccMailListField;
            }
            set
            {
                this.ccMailListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Mail", IsNullable = false)]
        public NotificationMail[] bccMailList
        {
            get
            {
                return this.bccMailListField;
            }
            set
            {
                this.bccMailListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NotificationMail
    {

        private string addressField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
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
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NotificationPhonePreset
    {

        private NotificationPhone[] phoneListField;

        private string contentField;

        private string titleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Phone", IsNullable = false)]
        public NotificationPhone[] PhoneList
        {
            get
            {
                return this.phoneListField;
            }
            set
            {
                this.phoneListField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NotificationPhone
    {

        private string numberField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
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
    }


}
