using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TWNewEgg.ActionFilters.Model
{
    public class VarnishHeads
    {
        [XmlElement("VarnishHead")]
        public List<VarnishHead> varnishHeads = new List<VarnishHead>();
    }
    public class VarnishHead
    {
        [XmlAttribute("controllerName")]
        public string controllerName { get; set; }

        [XmlAttribute("actionName")]
        public string actionName { get; set; }

        [XmlElement("ResponseHeads")]
        public List<VarnishKeyValue> responseHeads = new List<VarnishKeyValue>();
    }
    public class VarnishKeyValue
    {
        [XmlAttribute("headName")]
        public string headName { get; set; }

        [XmlAttribute("headValue")]
        public string headValue { get; set; }
    }
}
