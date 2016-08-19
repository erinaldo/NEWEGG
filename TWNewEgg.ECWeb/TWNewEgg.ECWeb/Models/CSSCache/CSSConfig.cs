using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TWNewEgg.ECWeb.Models.CSSCache
{
    public class CONSTNMAE
    {
        public const string CSSCONFIGNAME = "cssconfiglocation";
    }
    public class CSSConfig
    {
        [XmlElement("CSSFiles")]
        public List<CSSFile> cssFiles = new List<CSSFile>();
    }
    public class CSSFile
    {
        [XmlAttribute("key")]
        public string cssKeyName { get; set; }

        [XmlElement("Locations")]
        public List<CssLocations> cssLocations = new List<CssLocations>();
    }
    public class CssLocations
    {
        [XmlAttribute("value")]
        public string cssValue { get; set; }

    }
}