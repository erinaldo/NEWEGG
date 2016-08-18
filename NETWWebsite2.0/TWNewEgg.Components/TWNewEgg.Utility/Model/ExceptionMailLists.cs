using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TWNewEgg.Utility.Model
{
    public class MailLists
    {
        [XmlElement("MailGroup")]
        public List<MailGroup> MailGroups = new List<MailGroup>();
    }
    public class MailGroup
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("Mail")]
        public List<Mail> Mails = new List<Mail>();
    }
    public class Mail
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("address")]
        public string address { get; set; }
    }
}
